var v4 = require("uuid4");

var wsServer = require("ws").Server;
var wss = new wsServer({ port: 3333 });

console.log("Server opened on port 3333.");

var findMatchOp = "5";
var foundMatchOp = "6";

var ballPosChangeOp = "20";
var ballVelUpdateOp = "21";

var clientConnections = {};

wss.on("connection", function connection(client) {
  var playerUid = v4();
  clientConnections[playerUid] = client;
  client.id = playerUid;

  client.on("message", async function mss(receive) {
    receivedMessage = JSON.parse(receive);
    console.log(receivedMessage.opcode);

    if (receivedMessage.opcode == findMatchOp) {
      newMatchRequest(client.id);
    }

    if (receivedMessage.opcode == ballPosChangeOp) {
      sendBallPos(client.id, receivedMessage.message);
    }
  });

  client.on("close", function close() {
    console.log(`Client ${client.id} disconnected`);
    delete clientConnections[client.id];
    deleteMatchRequest(client.id);
  });
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
  console.log(players.length);
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
      JSON.stringify({
        uuid: players[0],
        opcode: foundMatchOp,
        gameRoom: newGameRoom,
      })
    );
    sendMessage(
      players[1],
      JSON.stringify({
        uuid: players[1],
        opcode: foundMatchOp,
        gameRoom: newGameRoom,
      })
    );
    players.splice(0, 2);
  }
}

function getInGameRoom(clientId) {
  return gameRooms.find(
    (gameRoom) => gameRoom.player1 == clientId || gameRoom.player2 == clientId
  );
}

function sendBallPos(fromClientId, message) {
  let gameRoom = getInGameRoom(fromClientId);
  let oponent =
    gameRoom.player1 == fromClientId ? gameRoom.player2 : gameRoom.player1;
  console.log(
    JSON.stringify({
      uuid: oponent,
      opcode: ballVelUpdateOp,
      vectorx: message.x,
      vectory: message.y,
    })
  );
  sendMessage(
    oponent,
    JSON.stringify({
      uuid: oponent,
      opcode: ballVelUpdateOp,
      vectorx: message.x,
      vectory: message.y,
    })
  );
}
