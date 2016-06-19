var net = require('net');
var fs = require('fs');
var iputils = require('./iputil');
var sab = require('./sab');

const ENCODING = "utf16le";
const CONFIGFILE = 'config.json';

var clients = [];
var maxplayers = 8;
var timer;
var config;


function socketConnect(socket) {
    logStatus(socket.remoteAddress + ':' + socket.remotePort + ' connected', 3);
}

function socketData(socket, data) {
    var recieved = new sab.Packet(data);
    logStatus('Recieved ' + data.length + ' Bytes from ' + socket.remoteAddress + ':' + socket.remotePort, 3);
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
            logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + socket.remoteAddress + ':' + socket.remotePort, 3);
            break;
        case sab.PacketType.INFO:
            // Will return information about the server.
            // Clients can request this to prevent unnecessary requests.
            
            var info = "Node Test Server;false;8;";
            var buf = Buffer.allocUnsafe(Buffer.byteLength(info, ENCODING))
            buf.write(info, 0, buf.length, ENCODING);
            packet = new sab.Packet(sab.PacketType.INFO_RESPONSE, buf);

            socket.write(packet.toBuffer());
            logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + socket.remoteAddress + ':' + socket.remotePort, 3);
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
                logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + socket.remoteAddress + ':' + socket.remotePort, 3);
                logStatus(socket.remoteAddress + ' failed to login, game full');
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
                logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + socket.remoteAddress + ':' + socket.remotePort, 3);
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
                logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + socket.remoteAddress + ':' + socket.remotePort, 3);
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
            var buf = Buffer.allocUnsafe(1 + Buffer.byteLength(socket.player.username, ENCODING));
            buf[0] = socket.player.id;
            buf.write(socket.player.username, 1, buf.length, ENCODING);
            
            // Send OK packet to player
            packet = new sab.Packet(sab.PacketType.LOGIN_OK, buf);
            socket.write(packet.toBuffer());
            logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + socket.remoteAddress + ':' + socket.remotePort, 3);
            
            // Notify all other players
            packet.flag = sab.PacketType.PLAYER_JOIN;
            clients.forEach(function (client) {
                client.write(packet.toBuffer());
                logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + client.remoteAddress + ':' + client.remotePort, 3);
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
            socket.player.velocity = new sab.Vector3(vel);
            socket.player.scale = new sab.Vector3(scl);
            
            sendPlayerUpdate(socket.player);
            break;
        case sab.PacketType.PLAYER_SHOOT:
            // Will be fired when someone shoots
            var hitsomeone = recieved.data[0];
            var pos = Buffer.allocUnsafe(12);
            recieved.data.copy(pos, 0, 1);
            var hitpoint = new sab.Vector3(pos);
            
            var buf = Buffer.allocUnsafe(14);
            buf[0] = hitsomeone;
            buf[1] = socket.player.id;
            hitpoint.toBuffer().copy(buf, 0, 2);
            
            // If someone was hit, add that information
            // to the packet and calculate the damage 
            if (hitsomeone) {
                var hitid = recieved.data[13];
                var amount = recieved.data[14];

                clients.forEach(function (client) {
                    if (hitid === client.player.id) {
                        var temp = Buffer.allocUnsafe(buf.length + 2);
                        buf.copy(temp, 0, 0);
                        temp[15] = client.id;
                        temp[16] = amount;
                        buf = temp;
                        
                        client.health -= amount;
                        // Check if the player died
                        if (client.player.health < 0) {
                            client.player.health = 0;
                            logStatus(client.player.username + ' got killed by ' + socket.player.username);
                        }
                        
                        sendPlayerUpdate(client.player);
                        return;
                    }
                });
            }
            
            // Send shoot packet
            packet = new sab.Packet(sab.PacketType.PLAYER_SHOOT, buf);
            clients.forEach(function (client) {
                if (client.player.id !== socket.player.id) {
                    client.write(packet.toBuffer());
                    logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + client.remoteAddress + ':' + client.remotePort, 3);
                }
            });

            break;
        default:
            var buf = Buffer.allocUnsafe(1);
            buf.writeInt8(sab.VERSION, 0);
            packet = new sab.Packet(sab.PacketType.PROTOCOL_NOT_SUPPORTED, buf);
            socket.write(packet.toBuffer());
            logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + socket.remoteAddress + ':' + socket.remotePort, 3);
            break;
    }
	
}

function socketDisconnect(socket, data) {
    if (socket.player !== undefined) {
        logStatus(socket.player.username + ' disconnected');
        // Remove client from the array
        clients = clients.filter(function (s) {
            return s.player.id !== socket.player.id;
        });
    }
    else {
        logStatus(socket.remoteAddress + ':' + socket.remotePort + ' disconnected', 3);
    }
}

function socketError(socket, data) {
    if (socket.player !== undefined) {
        logStatus(socket.player.username + ' disconnected with error:', 1);
        console.error(data.stack);
        // Remove client from the array
        clients = clients.filter(function (s) {
            return s.player.id !== socket.player.id;
        });
    }
    else {
        logStatus(socket.remoteAddress + ':' + socket.remotePort + ' disconnected with error', 3);
        console.error(data.stack);
    }
	
}

function sendPlayerUpdate(player) {
    var packet = new sab.Packet(sab.PacketType.PLAYER_UPDATE, player.toBuffer(ENCODING));
    clients.forEach(function (client) {
        if (client.player.id !== player.id) {
            client.write(packet.toBuffer());
            logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + client.remoteAddress + ':' + client.remotePort, 3);
        }
    });
}

function sendSync() {
    var buf = Buffer.allocUnsafe(0);
    clients.forEach(function (client) {
        var serialize = client.player.toBuffer(ENCODING);
        
        // Swap and add the length of serialized player
        var temp = Buffer.allocUnsafe(buf.length + serialize.length + 2);
        buf.copy(temp, 0, 0);
        temp.writeUInt16LE(serialize.length, buf.length);
        serialize.copy(temp, buf.length + 2, 0);
        buf = temp;
    });
    var packet = new sab.Packet(sab.PacketType.SYNC, buf);

    // Send to all clients
    clients.forEach(function (client) {
        client.write(packet.toBuffer());
        logStatus('Send ' + packet.toBuffer().length + ' Bytes to ' + client.remoteAddress + ':' + client.remotePort, 3);
    });
    
}

function logStatus(message, logtype) {
    if (logtype === undefined)
        logtype = 0;
    
    // Check if this type is supposed to get logged
    if (!config.log.info && logtype === 0) return;
    else if (!config.log.warning && logtype === 1) return;
    else if (!config.log.error && logtype === 2) return;
    else if (!config.log.network && logtype === 3) return;
    
    // Current time
    var output = '';
    if (config.log.time) {
        output += '[';
        var currentdate = new Date();
        output += currentdate.getHours() + ':' 
                + currentdate.getMinutes() + ':' 
                + currentdate.getSeconds() + '.' 
                + currentdate.getMilliseconds();
        output += ']';
        
    }
    
    // Color
    output += '[';
    if (config.log.color) {
        switch (logtype) {
            case 0: output += '\033[36m'; break;
            case 1: output += '\033[33m'; break;
            case 2: output += '\033[31m'; break;
            case 3: output += '\033[32m'; break;
            default:
        }
    }
    
    // Label
    switch (logtype) {
        case 0: output += 'Info'; break;
        case 1: output += 'Warning'; break;
        case 2: output += 'Error'; break;
        case 3: output += 'Network'; break;
        default:
    }
    
    if (config.log.color)
        output += '\033[0m';
    output += '] ' + message;

    console.log(output);
}

iputils.getPublicIP(function (err, data) {
    logStatus('Public IP: ' + data);
});

// Default Values
config = {
    log: {
        info: true,
        warning: true,
        error: true,
        file: {
            enabled: false,
            path: 'log.csv',
            type: 'csv'
        },
        time: true,
        network: false,
        color: true
    },
    name: 'SAB Server',
    port: 63001,
    sync: {
        enabled: true,
        timer: 1000
    }
};


try {
    var rconf = JSON.parse(fs.readFileSync(CONFIGFILE));
    
    // If values are not set, use defaults
    for (var option in config) {
        config[option] = rconf[option];
    }
    logStatus('Read configuration file');
} catch (e) {
    logStatus('Cannot read ' + CONFIGFILE + ', using defaults', 1);
}

// Print out general Information
logStatus('Version ' + sab.VERSION);
if (process.version !== 'v6.2.1') {
    logStatus('Wrong Node.js Version', 2);
    logStatus('Required: v6.2.1', 2);
    logStatus('Using: ' + process.version, 2);
    process.exit()
}
logStatus('Name: ' + config.name);
if (config.sync.enabled)
    logStatus('Sync enabled: ' + config.sync.timer + 'ms');
else
    logStatus('Sync disabled');

logStatus('Port: ' + config.port);

// Start listener
var localIP = iputils.getLocalIPSync();
net.createServer(function (socket) {
    
    socketConnect(socket);
    socket.on('data', function (data) {
        socketData(socket, data);
    });
    socket.on('end', function (data) {
        socketDisconnect(socket, data);
    });
    socket.on('error', function (data) {
        socketError(socket, data);
    });
			
}).listen(config.port, localIP);

logStatus('Started Listener');
logStatus('Local IP: ' + localIP);

// Start sync timer, if enabled
if (config.sync.enabled)
    timer = setInterval(sendSync, config.sync.timer);