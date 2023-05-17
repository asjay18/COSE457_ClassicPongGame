var v4 = require("uuid4");
const { newPlayer } = require("./manageGameRoom");

var wsServer = require("ws").Server;
var wss = new wsServer({ port: 3333 });

console.log("Server opened on port 3333.");

var findMatchOp = "5";

var clientConnections = {};

wss.on("connection", function connection(client) {
  client.id = v4();
  console.log(`Client ${client.id} connected`);
  clientConnections[client.id] = client;

  client.on("message", async function mss(receive) {
    receivedMessage = JSON.parse(receive);
    console.log(receivedMessage.opcode);

    if (receivedMessage.opcode == findMatchOp) {
      await newMatchRequest(client);
    }
  });

  client.on("close", function close() {
    console.log(`Client ${client.id} disconnected`);
    var clientId = client.id;
    delete clientConnections[clientId];
  });
  client.send("hello");
});

function newMatchRequest(client) {
  console.log(`client ${client.id} waiting for new match...`);
  newPlayer(client.id);
}

function sendMessage(clientId, message) {
  console.log(message);
  clientConnections[clientId].send(message);
}

module.exports = { sendMessage };
