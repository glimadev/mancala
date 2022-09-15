$(document).ready(function () {
    function handleError(error) {
        if (error.status == 400) {
            $(".error").html(error.responseText);
        }
    }

    function clearMessages() {
        $(".error").html(null);
        $(".success").html(null);
    }

    function setBoard(data) {
        $(".midsection-player1").html(null);
        $(".midsection-player2").html(null);

        var pos = 0;
        data.pits.forEach(pit => {
            if (pit.player == 1 && pit.isBigPit == false) $(".midsection-player1").prepend("<div class=\"pot pot-player1 current-player\" id=\"pt-" + pos + "\">" + pit.rocks + "</div>");
            if (pit.player == 2 && pit.isBigPit == false) $(".midsection-player2").append("<div class=\"pot pot-player2\" id=\"pb-" + pos + "\">" + pit.rocks + "</div>");
            pos++;
        });

        $(".endsection-player1").html("<div class=\"pot current-player\" id=\"bp1\">0</div>");
        $(".endsection-player2").html("<div class=\"pot\" id=\"bp2\">0</div>");
    }

    function newGame() {
        clearMessages();
        $.ajax({
            url: 'api/game',
            type: 'POST',
            success: function (data) {
                setBoard(data);
            },
            error: function (error) {
                handleError(error);
            }
        });
    }

    function move(pos) {
        clearMessages();
        $.ajax({
            url: 'api/game/' + pos,
            type: 'PUT',
            success: function () {
                getState();
            },
            error: function (error) {
                handleError(error);
            }
        });
    }

    function getState(isFirstCall) {
        $.ajax({
            url: 'api/game',
            type: 'GET',
            success: function (data) {

                if (data == null) {
                    newGame();
                    return;
                } else if (isFirstCall) {
                    setBoard(data);
                }

                var pos = 0;
                data.pits.forEach(pit => {
                    if (pit.player == 1 && pit.isBigPit == false) $("#pt-" + pos).html(pit.rocks);
                    if (pit.player == 2 && pit.isBigPit == false) $("#pb-" + pos).html(pit.rocks);
                    pos++;
                });

                $("#bp1").html(data.pits.filter(pit => pit.isBigPit && pit.player == 1)[0].rocks);
                $("#bp2").html(data.pits.filter(pit => pit.isBigPit && pit.player == 2)[0].rocks);

                $(".pot-player1").removeClass("current-player");
                $(".pot-player2").removeClass("current-player");

                $("#bp1").removeClass("current-player");
                $("#bp2").removeClass("current-player");

                if (data.currentPlayer == 1) {
                    $(".pot-player1").addClass("current-player");
                    $("#bp1").addClass("current-player");
                } else {
                    $(".pot-player2").addClass("current-player");
                    $("#bp2").addClass("current-player");
                }

                if (data.gameOver) {
                    if (data.winner !== null) {
                        $(".success").html("Player " + data.winner + " is the winner");
                    } else {
                        $(".success").html("Draw game!");
                    }

                    return;
                }
            },
            error: function (error) {
                handleError(error);
            }
        });
    }

    $(".midrow").on("click", "div", function (e) {
        if ($(".success").html() !== '') return;
        move(e.target.id.split('-')[1]);
    });

    $("#new-game").on("click", function () {
        newGame();
    });

    getState(true);
});