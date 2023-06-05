var uuid = require("uuid4");
var wsServer = require("ws").Server;

var wss = new wsServer({ port: 3333 });
console.log("Server opened on port 3333.");

var foundMatchOp = "6";
var playerReadyOp = "7";
var playerReadyResOp = "8";

var startGameOp = "10";

var ballPosChangeOp = "20";
var ballVelUpdateOp = "21";

var playerMovementOp = "31";
var oponentMovementOp = "32";

var oponentLeftOp = "90";

var clientConnections = {};

wss.on("connection", function connection(client) {
  var playerUid = uuid();

  clientConnections[playerUid] = client;
  client.id = playerUid;

  client.send(`{"id": "${client.id}"}`);
  newPlayer(client.id);

  client.on("message", async function mss(receive) {
    receivedMessage = JSON.parse(receive);

    if (receivedMessage.opcode == ballPosChangeOp) {
      sendBallPos(client.id, receivedMessage);
    } else if (receivedMessage.opcode == playerReadyOp) {
      checkPlayerReady(client.id);
    } else if (receivedMessage.opcode == playerMovementOp) {
      syncPlayerMovement(receivedMessage.playerData);
    }
  });

  client.on("close", function close() {
    console.log(`Client ${client.id} disconnected`);
    delete clientConnections[client.id];
    deletePlayerAndGameRoom(client.id);
  });
});

function sendMessage(clientId, message) {
  clientConnections[clientId].send(message);
}

var players = [];
var gameRooms = [];

function newPlayer(clientId) {
  players.push(clientId);
  checkPlayerList();
}

function deletePlayerAndGameRoom(clientId) {
  console.log(`client ${clientId} left the game...`);
  deletePlayer(clientId);
  try {
    const gameRoom = getInGameRoom(clientId);
    if (gameRoom != undefined) {
      deleteGameRoom(gameRoom.id);
    }
  } catch (error) {
    console.log("there's something wrong...");
    console.log(error);
  }
}

function deletePlayer(clientId) {
  var index = players.indexOf(clientId);
  if (index !== -1) {
    players.splice(index, 1);
  }
  console.log(players.length);
}

function deleteGameRoom(gameroomId) {
  const gameRoom = gameRooms.find((gameRoom) => gameRoom.id == gameroomId);
  var index = gameRooms.indexOf(gameRoom);
  if (index !== -1) {
    gameRooms.splice(index, 1);
  }
  try {
    sendMessage(
      gameRoom.player1,
      JSON.stringify({
        opcode: oponentLeftOp,
      })
    );
  } catch (error) {
    console.log("player1 already left");
  }
  try {
    sendMessage(
      gameRoom.player2,
      JSON.stringify({
        opcode: oponentLeftOp,
      })
    );
  } catch (error) {
    console.log("player2 already left");
  }
}

function checkPlayerList() {
  if (players.length >= 2) {
    var newGameRoom = {
      id: uuid(),
      status: "ready",
      p1status: false,
      p2status: false,
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
        opcode: foundMatchOp,
        gameRoom: newGameRoom,
      })
    );
    sendMessage(
      players[1],
      JSON.stringify({
        opcode: foundMatchOp,
        gameRoom: newGameRoom,
      })
    );
    players.splice(0, 2);
  }
}

function checkPlayerReady(clientId) {
  let gameRoom = getInGameRoom(clientId);
  let clientIsPlayer1 = gameRoom.player1 == clientId;

  if (clientIsPlayer1) {
    gameRoom.p1status = true;
  } else {
    gameRoom.p2status = true;
  }

  sendMessage(
    gameRoom.player1,
    JSON.stringify({
      opcode: playerReadyResOp,
      gameRoom: gameRoom,
    })
  );
  sendMessage(
    gameRoom.player2,
    JSON.stringify({
      opcode: playerReadyResOp,
      gameRoom: gameRoom,
    })
  );

  checkGameRoomReady(gameRoom);
}

function getInGameRoom(clientId) {
  return gameRooms.find(
    (gameRoom) => gameRoom.player1 == clientId || gameRoom.player2 == clientId
  );
}

function checkGameRoomReady(gameroom) {
  if (gameroom.p1status && gameroom.p2status) {
    console.log("gameroom " + gameroom.id + " start game!");
    sendMessage(
      gameroom.player1,
      JSON.stringify({
        opcode: startGameOp,
      })
    );
    sendMessage(
      gameroom.player2,
      JSON.stringify({
        opcode: startGameOp,
      })
    );
  }
}

function syncPlayerMovement(playerData) {
  let gameRoom = getInGameRoom(playerData.id);
  let oponentId;
  if (playerData.sideNumber == 1) {
    oponentId = gameRoom.player2;
  } else {
    oponentId = gameRoom.player1;
  }

  sendMessage(
    oponentId,
    JSON.stringify({
      opcode: oponentMovementOp,
      playerData: playerData,
    })
  );
}

function sendBallPos(fromClientId, message) {
  let gameRoom = getInGameRoom(fromClientId);
  let oponent =
    gameRoom.player1 == fromClientId ? gameRoom.player2 : gameRoom.player1;
  sendMessage(
    oponent,
    JSON.stringify({
      opcode: ballVelUpdateOp,
      vectorx: message.x,
      vectory: message.y,
    })
  );
}
