module.exports = {
	
	// Version needs to be updated if there were changes
    // to the protocol, to inform the client/server
	VERSION: 7,
	
	PacketType: Object.freeze({
			//Handshake
            HANDSHAKE : 20,
            HANDSHAKE_OK : 21,
            PROTOCOL_NOT_SUPPORTED : 22,

            //Info
            INFO : 40,
            INFO_RESPONSE : 41,

            //Login
            LOGIN : 60,
            LOGIN_OK : 61,
            USERNAME_TAKEN : 62,
            USERNAME_INVALID : 63,
            GAME_FULL : 64,
            BLACKLIST : 65,
            WHITELIST : 66,

            // Player
            PLAYER_JOIN : 80,
            PLAYER_UPDATE : 81,
            PLAYER_MOVE : 82,
            PLAYER_CROUCH : 83,
            PLAYER_SHOOT : 84,

            SYNC : 90,

            // Prop
            PROP_NEW : 100,
            PROP_CREATED : 101,
            PROP_MOVE : 102,
            PROP_DELETE : 103,
            PROP_UPDATE : 104,

            //Chat
            MESSAGE : 120,

            //Logout
            LOGOUT : 140,
            TIMEOUT : 141,
            KICK : 142,
            BAN : 143
    }),

	Packet: function (){
		
		if (arguments.length === 1) {
			
			if(Buffer.isBuffer(arguments[0])){
				// Parse bytes
				var buf = arguments[0];
				this.length = buf.readUInt16LE(0);
				this.flag = buf[2];
				
				this.data = Buffer.alloc(buf.length - 3);
				buf.copy(this.data, 0, 3, buf.length);
			}
			else{
				this.flag = arguments[0];
				this.data = Buffer.alloc(0);
				this.length = 1;
			}
			
			
		} else if (arguments.length === 2) {
			
			this.flag = arguments[0];
			this.data = arguments[1];
			this.length = this.data.length + 1;
			
		}
		else {
			
			this.flag = PacketType.HANDSHAKE;
			this.data = Buffer.alloc(0);
			this.length = 1;
			
		}
		
		this.toBuffer = function(){
			var buf = Buffer.allocUnsafe(this.length + 2);
			buf.writeUInt16LE(this.length, 0);
			buf[2] = this.flag;
			this.data.copy(buf, 3, 0, this.data.length);
			return buf;
		}
		
		this.toString = function(){
			for(var propertyName in module.exports.PacketType) {
				if(module.exports.PacketType[propertyName] == this.flag){
					return propertyName + ' [' + (this.length - 1) + ']';
				}
			}
			return 'UNKNOWN' + ' [' + (this.length - 1) + ']';
		}
		
		this.toBitString = function(){
            var output = "";
            this.toBuffer().forEach(function (bytevalue) {
                for (var i = 0; i < 8; i++)
                    output += (bytevalue >> i) & 1;
                output += ' ';
            });
			return output;
		}
	},
	
	Vector3: function (){
		
		this.x = 0;
		this.y = 0;
		this.z = 0;
		
		if(arguments.length === 1){
			var buffer = arguments[0];
			this.x = buffer.readFloatLE(0);
			this.y = buffer.readFloatLE(4);
			this.z = buffer.readFloatLE(8);
		}
		else if(arguments.length === 3){
			this.x = arguments[0];
			this.y = arguments[1];
			this.z = arguments[2];
		}
        
        this.toBuffer = function (){
            var buf = Buffer.allocUnsafe(12);
            buf.writeFloatLE(this.x, 0);
            buf.writeFloatLE(this.x, 4);
            buf.writeFloatLE(this.x, 8);
            return buf;
        }

		this.toString = function(){
			return '[' + this.x.toFixed(0) + ' ' + this.y.toFixed(0) + ' ' + this.z.toFixed(0) + ']';
		};
	},
	
	Player: function (username, id){
		
		this.username = username;
        this.id = id;
        this.teamid = 1;
        this.health = 100;
        this.armour = 50;
        this.crouching = false;
		
		var Vector3 = module.exports.Vector3;
		this.position = new Vector3();
        this.rotation = new Vector3();
        this.velocity = new Vector3();
		this.scale = new Vector3();
		
        this.toBuffer = function (encoding) {

            var userlen = Buffer.byteLength(this.username);
            if (Buffer.isEncoding(encoding))
                userlen = Buffer.byteLength(this.username, encoding);
            
            // 5(stats) + 48(transform) + userlen(username)
            var buf = Buffer.allocUnsafe(53 + userlen);
            buf[0] = this.id;
            buf[1] = this.teamid;
            buf[2] = this.health;
            buf[3] = this.armour;
            buf[4] = this.crouching;

            var pos = this.position.toBuffer();
            var rot = this.rotation.toBuffer();
            var vel = this.velocity.toBuffer();
            var scl = this.scale.toBuffer();
            
            pos.copy(buf, 5, 0);
            rot.copy(buf, 5, 0);
            vel.copy(buf, 5, 0);
            scl.copy(buf, 5, 0);

            buf.write(this.username, 53, userlen, encoding);
            return buf;
        };

        this.toString = function () {
            return '[' + this.id + '] ' + this.username + ' P' + this.position + ' R' + this.rotation + ' V' + this.velocity + ' S' + this.scale;
        };
	}
};
