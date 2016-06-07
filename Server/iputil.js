module.exports = {
	getLocalIP: function(cb){
		var os = require('os');
		var ifaces = os.networkInterfaces();
		var ip = false;
		Object.keys(ifaces).forEach(function (ifname) {

			ifaces[ifname].forEach(function (iface) {
				if ('IPv4' !== iface.family || iface.internal !== false) {
					// skip over internal (i.e. 127.0.0.1) and non-ipv4 addresses
					return;
				}
				
				ip = iface.address;
				return;
		  });
		});
		cb(false, ip);
		return ip;
	},
	getPublicIP: function(cb){
		var http = require('http');
		http.get('http://ip-api.com/json', function(res) {
			
			var data = '';

			res.on('data', function (chunk){
				data += chunk;
			});

			res.on('end',function(){
				var obj = JSON.parse(data);
				cb(false, obj.query);
			})

		}).on('error', function() {
			cb(true, null);
		});
	}
};