var net = require('net');
var iputils = require('./iputil');
var sab = require('./sab');

const PORT = 63001;
const ENCODING = "utf16le";

var clients = [];
var maxplayers = 8;

function socket_connect(socket) {
    console.log('CONNECTED: ' + socket.remoteAddress + ':' + socket.remotePort);
}

function socket_data(socket, data) {
    var recieved = new sab.Packet(data);
    // This will be the packet that we send back.
    var packet;
    switch (recieved.flag) {
        case sab.PacketType.HANDSHAKE:
            // Will check if client and server are using
            // the same protocol version.
            if (recieved.data.readInt8(0) === sab.VERSION)
                packet = new sab.Packet(sab.PacketType.HANDSHAKE_OK);
            else {
                var buf = Buffer.allocUnsafe(1);
                buf.writeInt8(sab.VERSION, 0);
                packet = new sab.Packet(sab.PacketType.PROTOCOL_NOT_SUPPORTED, buf);
            }
            socket.write(packet.toBuffer());
            break;
        case sab.PacketType.INFO:
            // Will return information about the server.
            // Clients can request this to prevent unnecessary requests.
            
            var info = "Node Test Server;false;8;";
            var buf = Buffer.allocUnsafe(Buffer.byteLength(info, ENCODING))
            buf.write(info, 0, buf.length, ENCODING);
            packet = new sab.Packet(sab.PacketType.INFO_RESPONSE, buf);
            socket.write(packet.toBuffer());
            break;
        case sab.PacketType.LOGIN:
            // Will check for:
            //  - Game is full
            //  - Username is taken by another player
            //  - Username is valid
            //    (Filter out inappropriate keywords/symbols/...)
            //  - Whitelist
            //  - Blacklist
            // 
            // If successfull, will add user to the player list.
            if (clients.length >= maxplayers) {
                packet = new sab.Packet(sab.PacketType.GAME_FULL);
                socket.write(packet.toBuffer());
                break;
            }
            
            var username = recieved.data.toString(ENCODING);
            
            // Check if another player is already using the given name.
            if (!clients.every(function (client) {
                if (client.player.username === username)
                    return false;
                return true;
            })) {
                packet = new sab.Packet(sab.PacketType.USERNAME_TAKEN);
                socket.write(packet.toBuffer());
                break;
            };
            
            // Obviously this will be replaced by something proper,
            // but for now this will do.
            var naughtyThings = ['hacker', 'badguy', 'IhateIceCream'];
            
            // Check if the username contains invalid things
            if (!naughtyThings.every(function (thing) {
                if (username.indexOf(thing) > -1)
                    return false;
                return true;
            })) {
                packet = new sab.Packet(sab.PacketType.USERNAME_INVALID);
                socket.write(packet.toBuffer());
                break;
            };
            
            
            // Check Whitelist
            
            // Check Blacklist
            
            
            // Everything is alright, so a new player object is created,
            // and a proper response will be send
            socket.player = new sab.Player(username, clients.length + 1);
            
            clients.push(socket);
            console.log(socket.player.username + ' connected...');
            
            
            // OK and JOIN packet share the same send data
            var buf = Buffer.allocUnsafe(1 + Buffer.byteLength(socket.player.username, ENCODING))
            buf.writeInt8(socket.player.id, 0);
            buf.write(socket.player.username, 1, buf.length, ENCODING);

            // Send OK packet to player
            packet = new sab.Packet(sab.PacketType.LOGIN_OK, buf);
            socket.write(packet.toBuffer());
            
            // Notify all other players
            packet.flag = sab.PacketType.PLAYER_JOIN;
            clients.forEach(function (client) {
                client.write(packet.toBuffer());
            });

            break;
        case sab.PacketType.PLAYER_MOVE:
            // Will be fired when the player moves, rotates, 
            // changes velocity or changes scale
            var pos = Buffer.allocUnsafe(12);
            var rot = Buffer.allocUnsafe(12);
            var vel = Buffer.allocUnsafe(12);
            var scl = Buffer.allocUnsafe(12);
            
            recieved.data.copy(pos, 0, 0);
            recieved.data.copy(rot, 0, 12);
            recieved.data.copy(vel, 0, 24);
            recieved.data.copy(scl, 0, 36);
            
            socket.player.position = new sab.Vector3(pos);
            socket.player.rotation = new sab.Vector3(rot);
            socket.player.veloctiy = new sab.Vector3(vel);
            socket.player.scale = new sab.Vector3(scl);
            
            sendPlayerUpdate(socket.player);
            break;
        default:
            var buf = Buffer.allocUnsafe(1);
            buf.writeInt8(sab.VERSION, 0);
            packet = new sab.Packet(sab.PacketType.PROTOCOL_NOT_SUPPORTED, buf);
            socket.write(packet.toBuffer());
            break;
    }
	
}

function socket_disconnect(socket, data) {
    if (socket.player !== undefined) {
        console.log(socket.player.username + ' disconnected...');
        // Remove client from the array
        clients = clients.filter(function (s) {
            return s.player.id !== socket.player.id;
        });
    }
    else {
        console.log('DISCONNECTED: ' + socket.remoteAddress + ':' + socket.remotePort);
    }
}

function socket_error(socket, data) {
    if (socket.player !== undefined) {
        console.log(socket.player.username + ' disconnected with error:');
        console.error(data);
        // Remove client from the array
        clients = clients.filter(function (s) {
            return s.player.id !== socket.player.id;
        });
    }
    else {
        console.log('DISCONNECTED WITH ERROR: ' + socket.remoteAddress + ':' + socket.remotePort);
        console.error(data);
    }
	
}

function sendPlayerUpdate(player){
    var packet = new sab.Packet(sab.PacketType.PLAYER_UPDATE, player.toBuffer());
    clients.forEach(function (client) {
        if (client.player.id !== player.id) {
            client.write(packet.toBuffer());
        }
    });
}

console.log('---Search and Betray Server V' + sab.VERSION + '---\r\n');
if (process.version !== 'v6.2.1') {
    console.error('Wrong Node.js Version', '\n\rRequired: v6.2.1', '\r\nUsing: ' + process.version);
    process.exit()
}

iputils.getLocalIP(function (err, localip) {
    iputils.getPublicIP(function (err, publicip) {
        
        console.log('Local IP: ' + localip);
        console.log('Public IP: ' + publicip);
        console.log('Port: ' + PORT);
        
        net.createServer(function (socket) {
            
            socket_connect(socket);
            socket.on('data', function (data) {
                socket_data(socket, data);
            });
            socket.on('end', function (data) {
                socket_disconnect(socket, data);
            });
            socket.on('error', function (data) {
                socket_error(socket, data);
            });
			
        }).listen(PORT, localip);
        
        console.log('Listening...');
    });
});