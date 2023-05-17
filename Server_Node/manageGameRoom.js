var players = [];
var gameRooms = [];

var isBusy = false;

function newPlayer(player) {
  players.push(player);
  var wait = true;
  while (wait) {
    if (!isBusy) {
      console.log("check!");
      checkPlayerList();
      break;
    }
  }
}

module.exports = { newPlayer };

const { sendMessage } = require("./mainServer");
var foundMatchOp = "6";

function checkPlayerList() {
  isBusy = true;
  if (players.length > 2) {
    var newGameRoom = {
      player1: players[0],
      player2: players[1],
      sets: 3,
      goal: 11,
      gameScore: (0, 0),
      setScore: (0, 0),
    };
    gameRooms.push(newGameRoom);
    sendMessage(
      players[0],
      JSON.stringify({ opcode: foundMatchOp, newGameRoom })
    );
    players.splice(0, 2);
  }
  isBusy = false;
}
