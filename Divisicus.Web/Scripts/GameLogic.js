﻿var countdown;
var round = 1;
var multiple1;
var multiple2;
initButtons();

function initButtons() {
    var numbers = [2, 3, 4, 5, 6, 7, 8, 9, 11, 13, 17, 19, 21, 23, 27, 29, 31, 33, 37, 39, 41, 43, 47, 49, 51, 53, 57, 59];
    for (var i = 0; i < 12; i++) {
        viewModel.numbersInPlay.push(numbers[i + viewModel.level]);
    }
};

function beginSalvo() {
    $('#startButton').addClass('hidden');
    startDelay();
}

function setTarget() {
    multiple1 = viewModel.numbersInPlay()[(Math.floor(Math.random() * 6))];
    multiple2 = viewModel.numbersInPlay()[(Math.floor(Math.random() * 6)) + 6];
    viewModel.target(multiple1 * multiple2);
};

function startTimer() {
    var timer = 30.0;
    countdown = setInterval(function () {
        timer -= .1;
        document.getElementById("timer").innerHTML = (Math.round(timer * 10) / 10).toFixed(1);
        if (timer <= 0) {
            clearInterval(countdown);
            document.getElementById("timer").innerHTML = "0.0";
        };
    }, 100);
};

function selectNumber() {
    if (viewModel.target() == " ") return;
    $(this).toggleClass('btn-primary');
    if (viewModel.selections()[0] == this.value) {
        viewModel.selections.removeAll();
    } else viewModel.selections.push(this.value);
    if (viewModel.selections().length == 2) {
        testSelections();
    };
};

function testSelections() {
    var time = document.getElementById("timer").innerHTML;
    var num1 = viewModel.selections()[0];
    var num2 = viewModel.selections()[1];
    if (num1 * num2 == viewModel.target()) {
        viewModel.score(viewModel.score() + awardPoints(time));
    }
    else {
        alert("Incorrect\nMultiples: " + multiple1 + " & " + multiple2);
    }
    viewModel.selections.removeAll();
    resetButtons();
    clearInterval(countdown);
    nextRound();
};

function nextRound() {
    if (round == 6) {
        endGame();
        return;
    }
    else round++;
    blurButtons();
    startDelay();
}

function startDelay() {
    viewModel.target("Ready...");
    setTimeout(function () {
        viewModel.target("Set...");
    }, 1000);
    setTimeout(function () {
        setTarget();
        startTimer();
    }, 2000);
}

function blurButtons() {
    //removes the focus (and thus the styling) between rounds
    $('.btn-fixed').each(function(){this.blur()});
}

function endGame() {
    viewModel.target(" ");
    var url = "/Home/DebriefGame";
    var user = "@Model.UserId";
    var score = viewModel.score;
    $.post(url, { user: user, score: score }, function (data) {
        alert(data);
        location.reload();
    });
}

function resetButtons() {
    $("input").removeClass("btn-primary");
}

function awardPoints(time) {
    var score = viewModel.numbersInPlay()[5] * viewModel.numbersInPlay()[11];
    if(time==0) multiplier = 0.5;
    else var multiplier = Math.floor((Math.pow(1.1, time * 1.4) * .11 + 1) * 10) / 10;
    var totalScore = Math.floor(score * multiplier);
    alert("Level " + viewModel.level + " Score: " + score + "\nTime Multiplier: " + multiplier
        + "\nTotal Score: " + totalScore);
    return totalScore;
}

ko.applyBindings(viewModel);