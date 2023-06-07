var uuid = require("uuid4");
var wsServer = require("ws").Server;

var wss = new wsServer({ port: 3333 });
console.log("Server opened on port 3333.");

var normalGameMatchOp = "0";
var friendGameMatchOp = "1";
var friendGameMatchClientOp = "2";

var friendGameRoomNumberOp = "3";

var joinGameErrorOp = "22"; //TODO

var requestCheckRoomNumberOp = "4";
var checkedRoomNumberOp = "5";

var foundMatchOp = "6";
var playerReadyOp = "7";
var playerReadyResOp = "8";

var startGameOp = "10";

var playerMovementOp = "30";
var oponentMovementOp = "31";

var playerScoresOp = "40";
var oponentScoresOp = "41";

var playerHitsOp = "50";
var oponentHitsOp = "51";

var oponentLeftOp = "90";
var setEndOp = "98";
var gameEndOp = "99";

var clientConnections = {};

wss.on("connection", function connection(client) {
  var playerUid = uuid();
  console.log(`new player ${playerUid}`);

  clientConnections[playerUid] = client;
  client.id = playerUid;

  client.on("message", async function mss(receive) {
    receivedMessage = JSON.parse(receive);

    if (receivedMessage.opcode == playerReadyOp) {
      checkPlayerReady(client.id);
    } else if (receivedMessage.opcode == playerMovementOp) {
      syncPlayerMovement(receivedMessage.playerData);
    } else if (receivedMessage.opcode == playerScoresOp) {
      playerScores(client.id);
    } else if (receivedMessage.opcode == playerHitsOp) {
      playerHits(client.id, receivedMessage.ballData);
    } else if (receivedMessage.opcode == normalGameMatchOp) {
      client.send(`{"id": "${client.id}"}`);
      newPlayer(client.id);
    } else if (receivedMessage.opcode == friendGameMatchOp) {
      client.send(`{"id": "${client.id}"}`);
      makeFriendGameRoom(client.id);
    } else if (receivedMessage.opcode == friendGameMatchClientOp) {
      client.send(`{"id": "${client.id}"}`);
      joinFriendGameRoom(client.id, receivedMessage.message);
    } else if (receivedMessage.opcode == requestCheckRoomNumberOp) {
      findFriendGameRoom(client.id, receivedMessage.message);
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

function getInGameRoom(clientId) {
  return gameRooms.find(
    (gameRoom) => gameRoom.player1 == clientId || gameRoom.player2 == clientId
  );
}

function newPlayer(clientId) {
  players.push(clientId);
  checkPlayerList();
}

function deletePlayerAndGameRoom(clientId) {
  console.log(`client ${clientId} left the game...`);
  try {
    const gameRoom = getInGameRoom(clientId);
    if (gameRoom != undefined) {
      deleteGameRoom(gameRoom.id);
    }
  } catch (error) {
    console.log("there's something wrong...");
    console.log(error);
  }
  try {
    deletePlayer(clientId);
  } catch (error) {
    console.log(error);
  }
}

function deletePlayer(clientId) {
  var index = players.indexOf(clientId);
  if (index !== -1) {
    players.splice(index, 1);
  }
}

function deleteGameRoom(gameroomId) {
  const gameRoom = gameRooms.find((gameRoom) => gameRoom.id == gameroomId);

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

  var index = gameRooms.indexOf(gameRoom);
  if (index !== -1) {
    gameRooms.splice(index, 1);
  }
}

const generateRandomString = (length) => {
  let result = "";
  const characters =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
  const charactersLength = characters.length;
  for (let i = 0; i < length; i++) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }
  return result;
};

function makeFriendGameRoom(clientId) {
  var newGameRoom = {
    id: generateRandomString(8),
    status: "ready",
    p1status: false,
    p2status: false,
    player1: clientId,
    player2: "none",
    goal: 11,
    sets: 3,
    p1GameScore: 0,
    p2GameScore: 0,
    p1SetScore: 0,
    p2SetScore: 0,
  };
  gameRooms.push(newGameRoom);
  console.log(`New friend game room : ${newGameRoom.id}`);
  sendMessage(
    clientId,
    JSON.stringify({
      opcode: friendGameRoomNumberOp,
      message: newGameRoom.id,
    })
  );
}

function findFriendGameRoom(clientId, gameRoomId) {
  if (gameRoomId.length != 8) {
    sendMessage(
      clientId,
      JSON.stringify({
        opcode: checkedRoomNumberOp,
        message: "gameroom code should have 8 digits",
      })
    );
    return;
  }
  var gameRoom = gameRooms.find((gameRoom) => gameRoom.id == gameRoomId);
  if (gameRoom == undefined) {
    sendMessage(
      clientId,
      JSON.stringify({
        opcode: checkedRoomNumberOp,
        message: "there's no such room",
      })
    );
    return;
  }

  sendMessage(
    clientId,
    JSON.stringify({
      opcode: checkedRoomNumberOp,
      message: "join",
      gameRoom: gameRoom,
    })
  );
}

function joinFriendGameRoom(clientId, gameRoomId) {
  var gameRoom = gameRooms.find((gameRoom) => gameRoom.id == gameRoomId);

  if (gameRoom == undefined) {
    sendMessage(
      clientId,
      JSON.stringify({
        opcode: joinGameErrorOp,
        message: "there's no such room",
      })
    );
    return;
  }

  if (gameRoom.player2 != "none") {
    sendMessage(
      clientId,
      JSON.stringify({
        opcode: joinGameErrorOp,
        message: "the room already has 2 players",
      })
    );
    return;
  }

  console.log(`Client ${clientId} join GameRoom ${gameRoom.id}`);
  gameRoom.player2 = clientId;

  sendMessage(
    gameRoom.player2,
    JSON.stringify({
      opcode: foundMatchOp,
      gameRoom: gameRoom,
    })
  );
  sendMessage(
    gameRoom.player1,
    JSON.stringify({
      opcode: foundMatchOp,
      gameRoom: gameRoom,
    })
  );
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
      goal: 11,
      sets: 3,
      p1GameScore: 0,
      p2GameScore: 0,
      p1SetScore: 0,
      p2SetScore: 0,
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
  var gameRoom;
  try {
    gameRoom = getInGameRoom(playerData.id);
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
  } catch (error) {
    console.log("[ERROR] error in synchronizing player movement");
    console.log(error);
    // sendMessage(
    //   playerData.id,
    //   JSON.stringify({
    //     opcode: oponentLeftOp,
    //   })
    // );
  }
}

function playerScores(clientId) {
  let gameRoom = getInGameRoom(clientId);
  let oponentId;

  if (gameRoom.player1 == clientId) {
    oponentId = gameRoom.player2;
    setGameRoomScore(1, gameRoom.id);
  } else {
    oponentId = gameRoom.player1;
    setGameRoomScore(2, gameRoom.id);
  }

  sendMessage(
    oponentId,
    JSON.stringify({
      opcode: oponentScoresOp,
    })
  );
}

function setGameRoomScore(scoredPlayer, gameRoomId) {
  let gameRoom = gameRooms.find((gameRoom) => gameRoom.id == gameRoomId);

  if (scoredPlayer == 1) {
    gameRoom.p1GameScore += 1;
    if (gameRoom.p1GameScore >= gameRoom.goal) {
      gameRoom.p1GameScore = 0;
      gameRoom.p2GameScore = 0;
      gameRoom.p1SetScore += 1;
      if (gameRoom.p1SetScore >= gameRoom.sets / 2) {
        gameEnds(1, gameRoom);
      } else {
        setEnds(1, gameRoom);
      }
    }
  } else {
    gameRoom.p2GameScore += 1;
    console.log(`gameRoom player 2 score is now ${gameRoom.p2GameScore}`);
    if (gameRoom.p2GameScore >= gameRoom.goal) {
      gameRoom.p1GameScore = 0;
      gameRoom.p2GameScore = 0;
      gameRoom.p2SetScore += 1;
      if (gameRoom.p2SetScore >= gameRoom.sets / 2) {
        gameEnds(2, gameRoom);
      } else {
        setEnds(2, gameRoom);
      }
    }
  }
}

function playerHits(clientId, ballData) {
  let gameRoom = getInGameRoom(clientId);
  let oponentId;

  if (gameRoom.player1 == clientId) {
    oponentId = gameRoom.player2;
  } else {
    oponentId = gameRoom.player1;
  }

  sendMessage(
    oponentId,
    JSON.stringify({
      opcode: oponentHitsOp,
      ballData: ballData,
    })
  );
}

function gameEnds(winner, gameRoom) {
  console.log(`final winner is player ${winner}!`);
  sendMessage(
    gameRoom.player1,
    JSON.stringify({
      opcode: gameEndOp,
      message: `PLAYER ${winner}`,
    })
  );
  sendMessage(
    gameRoom.player2,
    JSON.stringify({
      opcode: gameEndOp,
      message: `PLAYER ${winner}`,
    })
  );
}

function setEnds(winner, gameRoom) {
  console.log(`winner is player ${winner}!`);
  sendMessage(
    gameRoom.player1,
    JSON.stringify({
      opcode: setEndOp,
      message: `PLAYER ${winner}`,
    })
  );
  sendMessage(
    gameRoom.player2,
    JSON.stringify({
      opcode: setEndOp,
      message: `PLAYER ${winner}`,
    })
  );
}
