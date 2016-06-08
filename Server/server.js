var net = require('net');
var fs = require('fs');
var iputils = require('./iputil');
var sab = require('./sab');

const PORT = 63001;
const ENCODING_UNICODE = "utf16le";

var clients = [];
var maxplayers = 8;
var timer;

function socket_connect(socket) {
    //logStatus(socket.remoteAddress + ':' + socket.remotePort + ' connected...');
}

function socket_data(socket, data) {
    var recieved = new sab.Packet(data);
    // This will be the packet that we send back.
    var packet;
    switch (recieved.flag) {
        case sab.PacketType.HANDSHAKE:
            // Will check if client and server are using
            // the same protocol version.
            var userversion = recieved.data.readInt8(0);
            if (userversion === sab.VERSION) {
                packet = new sab.Packet(sab.PacketType.HANDSHAKE_OK);
                logStatus('Successful Handshake with ' + socket.remoteAddress);
            }
            else {
                var buf = Buffer.allocUnsafe(1);
                buf.writeInt8(sab.VERSION, 0);
                packet = new sab.Packet(sab.PacketType.PROTOCOL_NOT_SUPPORTED, buf);
                logStatus('Failed Handshake with ' + socket.remoteAddress + ' used version ' + userversion, 1);
            }
            socket.write(packet.toBuffer());
            break;
        case sab.PacketType.INFO:
            // Will return information about the server.
            // Clients can request this to prevent unnecessary requests.
            
            var info = "Node Test Server;false;8;";
            var buf = Buffer.allocUnsafe(Buffer.byteLength(info, ENCODING_UNICODE))
            buf.write(info, 0, buf.length, ENCODING_UNICODE);
            packet = new sab.Packet(sab.PacketType.INFO_RESPONSE, buf);
            socket.write(packet.toBuffer());
            logStatus(socket.remoteAddress + ' requested info');
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
                logStatus(socket.remoteAddress + ' failed to login, game full');
                break;
            }
            
            var username = recieved.data.toString(ENCODING_UNICODE);
            
            // Check if another player is already using the given name.
            if (!clients.every(function (client) {
                if (client.player.username === username)
                    return false;
                return true;
            })) {
                packet = new sab.Packet(sab.PacketType.USERNAME_TAKEN);
                socket.write(packet.toBuffer());
                logStatus(socket.remoteAddress + ' failed to login, username "' + username + '" already taken');
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
                logStatus(socket.remoteAddress + ' failed to login, username "' + username + '" invalid');
                break;
            };
            
            
            // Check Whitelist
            
            // Check Blacklist
            
            
            // Everything is alright, so a new player object is created,
            // and a proper response will be send
            socket.player = new sab.Player(username, clients.length + 1);
            
            clients.push(socket);
            logStatus(socket.remoteAddress + ' logged in as ' + socket.player.username);
            
            
            // OK and JOIN packet share the same send data
            var buf = Buffer.allocUnsafe(1 + Buffer.byteLength(socket.player.username, ENCODING_UNICODE))
            buf.writeInt8(socket.player.id, 0);
            buf.write(socket.player.username, 1, buf.length, ENCODING_UNICODE);
            
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
        logStatus(socket.player.username + ' disconnected');
        // Remove client from the array
        clients = clients.filter(function (s) {
            return s.player.id !== socket.player.id;
        });
    }
    else {
        //logStatus(socket.remoteAddress + ':' + socket.remotePort + ' disconnected');
    }
}

function socket_error(socket, data) {
    if (socket.player !== undefined) {
        logStatus(socket.player.username + ' disconnected with error:', 1);
        console.error(data);
        // Remove client from the array
        clients = clients.filter(function (s) {
            return s.player.id !== socket.player.id;
        });
    }
    else {
        logStatus(socket.remoteAddress + ':' + socket.remotePort + ' disconnected with error:', 1);
        console.error(data);
    }
	
}

function sendPlayerUpdate(player) {
    var packet = new sab.Packet(sab.PacketType.PLAYER_UPDATE, player.toBuffer());
    clients.forEach(function (client) {
        //if (client.player.id !== player.id) {
            client.write(packet.toBuffer());
        //}
    });
}

function sendSync() {
    //logStatus('Send sync packet');
}

function logStatus(message, errorlevel) {
    if (errorlevel === undefined)
        errorlevel = 0;
    var output = '[';
    switch (errorlevel) {
        case 0:
            output += '\033[36mInfo';
            break;
        case 1:
            output += '\033[33mWarning';
            break;
        case 2:
            output += '\033[31mError';
            break;
        default:
    }
    output += '\033[0m] ' + message;
    console.log(output);
}

logStatus('Search and Betray Server, Version ' + sab.VERSION);
if (process.version !== 'v6.2.1') {
    logStatus('Wrong Node.js Version', 2);
    logStatus('Required: v6.2.1', 2);
    logStatus('Using: ' + process.version, 2);
    process.exit()
}

iputils.getLocalIP(function (err, localip) {
    iputils.getPublicIP(function (err, publicip) {
        
        var config = {
            name: 'SAB Server',
            port: 63001,
            synctimer: 1000
        };
        var configfile = 'config.json';
        try {
            var rconf = JSON.parse(fs.readFileSync(configfile));
            config.name = rconf.name !== undefined ? rconf.name : config.name;
            config.port = rconf.port !== undefined ? rconf.port : config.port;
            config.synctimer = rconf.synctimer !== undefined ? rconf.synctimer : config.synctimer;
            logStatus('Read configuration file');
        } catch (e) {
            logStatus('Cannot read ' + configfile + ', using defaults', 1);
        }
        
        logStatus('Name: ' + config.name);
        if (config.synctimer === 0)
            logStatus('Sync disabled');
        else
            logStatus('Sync enabled: ' + config.synctimer + 'ms');
        logStatus('Local IP: ' + localip);
        logStatus('Public IP: ' + publicip);
        logStatus('Port: ' + PORT);
        
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
        
        logStatus('Listening...');
        if (config.synctimer !== 0)
            timer = setInterval(sendSync, config.synctimer);
    });
});