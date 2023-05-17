var v4 = require("uuid4");

var wsServer = require("ws").Server;
var wss = new wsServer({ port: 3333 });

console.log("Server opened on port 3333.");

var findMatchOp = "5";
var foundMatchOp = "6";

var clientConnections = {};

wss.on("connection", function connection(client) {
  client.id = v4();
  console.log(`Client ${client.id} connected`);
  clientConnections[client.id] = client;

  client.on("message", async function mss(receive) {
    receivedMessage = JSON.parse(receive);
    console.log(receivedMessage.opcode);

    if (receivedMessage.opcode == findMatchOp) {
      newMatchRequest(client);
    }
  });

  client.on("close", function close() {
    console.log(`Client ${client.id} disconnected`);
    var clientId = client.id;
    delete clientConnections[clientId];
    deleteMatchRequest(clientId);
  });
  client.send("hello");
});

function newMatchRequest(clientId) {
  console.log(`client ${clientId} waiting for new match...`);
  newPlayer(clientId);
}

function deleteMatchRequest(clientId) {
  console.log(`client ${clientId} left the game...`);
  deletePlayer(clientId);
}

function sendMessage(clientId, message) {
  console.log(message);
  clientConnections[clientId].send(message);
}

var players = [];
var gameRooms = [];

var isBusy = false;

function newPlayer(clientId) {
  players.push(clientId);
  checkPlayerList();
}

function deletePlayer(clientId) {
  var index = players.indexOf(clientId);
  if (index !== -1) {
    players.splice(index, 1);
  }
}

function checkPlayerList() {
  if (players.length >= 2) {
    var newGameRoom = {
      player1: players[0],
      player2: players[1],
      sets: 3,
      goal: 11,
      gameScore: (0, 0),
      setScore: (0, 0),
    };
    gameRooms.push(newGameRoom);
    console.log("gameroom established!");
    sendMessage(
      players[0],
      JSON.stringify({ opcode: foundMatchOp, newGameRoom })
    );
    players.splice(0, 2);
  }
}
