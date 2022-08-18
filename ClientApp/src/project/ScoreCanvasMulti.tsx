import React, { useRef, useEffect, useState, useMemo } from 'react';
import { Helmet } from 'react-helmet';
import mystyles from './bugs.module.css';

interface TeamBools {
  home: boolean,
  guest: boolean
}

interface TeamStrings {
  home: string,
  guest: string
}

interface TeamNumbers {
  home: number,
  guest: number
}

interface FoulInfo {
  fouls: TeamNumbers,
  bonus: TeamStrings
}

interface DownInfo {
  down: string,
  distance: number,
  ballon: number
}

interface Clocks {
  game: string,
  timeout: string,
  play: string
}

const ScoreCanvas = () => {

  const flagCanvasRef = useRef() as React.MutableRefObject<HTMLCanvasElement>;
  const scoreCanvasRef = useRef() as React.MutableRefObject<HTMLCanvasElement>;
  const toCanvasRef = useRef() as React.MutableRefObject<HTMLCanvasElement>;
  const [names, setNames] = useState<TeamStrings>({home: "HOME", guest: "GUEST"});
  const [scores, setScores] = useState<TeamNumbers>({home: -1, guest: -1});
  const [clocks, setClocks] = useState<Clocks>({game: "-:--", timeout: "", play: ""});
  const [quarter, setQuarter] = useState<string>("");
  const [tols, setTols] = useState<TeamNumbers>({home: 0, guest: 0});
  const [poss, setPoss] = useState<TeamBools>({home: false, guest: false});
  const [ballInfo, setBallInfo] = useState<DownInfo>({down: "", distance: 0, ballon: 0});
  const [hasDnD, setHasDnD] = useState<boolean>(false);
  const [flag, setFlag] = useState<boolean>(false);
  const [timeOuts, setTimeOuts] = useState<TeamBools>({home: false, guest: false});
  const [colors, setColors] = useState<TeamStrings>({home: "red", guest: "blue"});
  const [records, setRecords] = useState<TeamStrings>({home: "", guest: ""});
  const [sport, setSport] = useState<string>("Default");

  const [foulInfo, setFoulInfo] = useState<FoulInfo>({
    fouls: {home: 0, guest: 0},
    bonus: {home: "", guest: ""}
  });

  function ordinal(n: number) {
    var s = ["th", "st", "nd", "rd"];
    var v = n%100;
    return n + (s[(v-20)%10] || s[v] || s[0]);
  }

  const draw = (ctx: CanvasRenderingContext2D) => {
    // console.log("inside of draw()");
    setupScoreboard(ctx);
    setupGame(ctx);
  };

  function setupScoreboard(ctx: CanvasRenderingContext2D) {
    // Draw the basic background for the scoreboard
    if (ctx) {
      console.log("Setup scoreboard");
      ctx.fillStyle = "#30303080";
      ctx.fillRect(30, 2, 819, 75); // Main background
      ctx.fillStyle = "#525252cc";
      ctx.fillRect(253, 2, 100, 75); // Home score background
      ctx.fillRect(614, 2, 100, 75); // guest score background
    }
  }

  useEffect(() => {
    // TODO: Reset TOL and foul panels
      // console.log("Resetting Foul panel");
      var c = scoreCanvasRef.current;
      var ctx = c.getContext("2d");
      if (ctx) {
        // Reset the bottom fouls/TOL panel
        ctx.clearRect(30, 52, 223, 25);
        ctx.clearRect(391, 52, 223, 25);
        ctx.fillStyle = "#30303080";
        ctx.fillRect(30, 52, 223, 25); // Main background
        ctx.fillRect(391, 52, 223, 25); // Main background
        ctx.fillStyle = colors.home;
        ctx.fillRect(30, 52, 223, 25);
        ctx.fillStyle = colors.guest;
        ctx.fillRect(391, 52, 223, 25);
        drawGuestIcon();
        drawHomeIcon();
        // Rest the right edge foul panel
        ctx.clearRect(239, 2, 14, 75);
        ctx.clearRect(600, 2, 14, 75);
        ctx.fillStyle = "#30303080";
        ctx.fillRect(239, 2, 14, 75);
        ctx.fillRect(600, 2, 14, 75);
        ctx.fillStyle = colors.home;
        ctx.fillRect(239, 2, 14, 75);
        ctx.fillStyle = colors.guest;
        ctx.fillRect(600, 2, 14, 75);
      }
  }, [sport, colors]);

  // function drawFoulsPanels(homeFouls, homeBonus, homePoss, awayFouls, awayBonus, awayPoss) {
  useEffect(() => {
    // console.log("Drawing Foul panel");
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx && sport === "Basketball") {
      // Reset the backgrounds
      ctx.clearRect(30, 52, 223, 25);
      ctx.clearRect(391, 52, 223, 25);
      ctx.fillStyle = "#30303080";
      ctx.fillRect(30, 52, 223, 25); // Main background
      ctx.fillRect(391, 52, 223, 25); // Main background
      ctx.fillStyle = colors.home;
      ctx.fillRect(30, 52, 223, 25);
      ctx.fillStyle = colors.guest;
      ctx.fillRect(391, 52, 223, 25);
      drawHomeIcon();
      drawGuestIcon();
      ctx.font = "12pt Segoe UI Semibold";
      // Write fouls
      ctx.fillStyle = "white";
      ctx.fillText("FOULS: " + foulInfo.fouls.home, 160, 70);
      ctx.fillText("FOULS: " + foulInfo.fouls.guest, 522, 70);
      // Write Bonus indicators
      ctx.fillText((foulInfo.bonus.home) ? "BONUS" : "", 85, 70);
      ctx.fillText((foulInfo.bonus.guest) ? "BONUS" : "", 447, 70);
      // Write the Possession indicators
      ctx.fillStyle = (poss.home) ? "yellow" : colors.home;
      ctx.fillText("P", 240, 70);
      ctx.fillStyle = (poss.guest) ? "yellow" : colors.guest;
      ctx.fillText("P", 602, 70);
    }
  }, [sport, colors, poss, foulInfo]);
//  }, [foulInfo, sport, poss, colors]);

  // function scores(homeScore, guestScore) {
  useEffect(() => {
    // console.log("Score update - home: %d, guest: %d", scores.home, scores.guest);
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.clearRect(253, 2, 100, 75);
      ctx.clearRect(614, 2, 100, 75);
      ctx.fillStyle = "#525252cc";
      ctx.fillRect(253, 2, 100, 75); // Home score background
      ctx.fillRect(614, 2, 100, 75); // guest score background
      ctx.font = "52pt Segoe UI Black";
      ctx.fillStyle = "white";
      ctx.fillText(scores.home.toString(), 303 - (ctx.measureText(scores.home.toString()).width/2), 65);
      ctx.fillText(scores.guest.toString(), 664 - (ctx.measureText(scores.guest.toString()).width/2), 65);
    }
  }, [scores]);

  // function quarter(quarter) {
  useEffect(() => {
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.clearRect(714, 2, 135, 38);
      ctx.fillStyle = "#282828cc";
      ctx.fillRect(714, 2, 135, 38); // Period/Time background
      ctx.font = "24pt Segoe UI Semibold";
      if (sport === "Football") {
        ctx.fillStyle = "yellow";
        ctx.fillText(quarter, 783 - (ctx.measureText(quarter).width/2), 35);
      } else if (sport === "Basketball") {
        ctx.fillStyle = "white";
        ctx.fillText(quarter, 817 - (ctx.measureText(quarter).width/2), 35);
        ctx.fillStyle = "yellow";
        // console.log("Playclock: %s", clocks.play);
        if (clocks.play) {
            ctx.fillText(clocks.play, 750 - (ctx.measureText(clocks.play).width/2), 35);
        }
      }
    }
  }, [quarter, clocks, sport]);

  // function clock(clock) {
  useEffect(() => {
    // console.log("Clock update: %s", clocks.game);
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.clearRect(714, 39, 135, 38);
      ctx.fillStyle = "#282828cc";
      ctx.fillRect(714, 39, 135, 38); // Period/Time background
      ctx.font = "24pt Segoe UI Semibold";
      if (clocks.game !== "-:--") {
          ctx.fillStyle = "white";
          ctx.fillText(clocks.game, 784 - (ctx.measureText(clocks.game).width/2), 70);
      }
    }
  }, [clocks.game]);

  // function timeOuts(home, guest) {
  useEffect(() => {
    // console.log("Drawing TOL panel");
    if (sport !== "Default") {
      var c = scoreCanvasRef.current;
      var ctx = c.getContext("2d");
      var noTO = "#525252ff";
      var hasTO = "#ffff00ff";
      if (ctx) {
        ctx.lineWidth = 1;
        ctx.lineCap = "round";
        // Set to Football by default, change if Basketball
        var startX = 0;
        var endX = 145;
        var startCount = 1;
        var endCount = 3;
        var inc = 1;
        // var x = 0;
        var startY = 0;
        var endY = 60;
        var xGap = 15;
        var yGap = 0;
        var xInc = 10;
        var yInc = 0;
        if (sport === "Basketball") {
          endX = 245;
          startCount = 4;
          endCount = 1;
          inc = -1;
          // x = 245; // Home team
          startY = 0;
          endY = -1;
          xGap = 0;
          yGap = 11;
          xInc = 0;
          yInc = 0;
        }
        for (let i=startCount; i!==endCount+inc; i=i+inc) {
            ctx.strokeStyle = (tols.home >= i) ? hasTO : noTO;
            startX = endX + xGap;
            startY = endY + yGap;
            endX = startX + xInc;
            endY = startY + yInc;
            ctx.beginPath();
            ctx.moveTo(startX, startY);
            ctx.lineTo(endX, endY);
            ctx.arc(endX, endY+3, 3, 1.5*Math.PI, 0.5*Math.PI, false);
            ctx.lineTo(startX, startY+6);
            ctx.arc(startX, startY+3, 3, 0.5*Math.PI, 1.5*Math.PI, false);
            ctx.closePath();
            ctx.fillStyle = ctx.strokeStyle;
            ctx.fill();
            ctx.stroke();
        }
        if (sport === "Football") {
          endX = 506;
        } else {
          endX = 607;
          endY = -2;
        }
        for (let i=startCount; i!==endCount+inc; i=i+inc) {
            ctx.strokeStyle = (tols.guest >= i) ? hasTO : noTO;
            startX = endX + xGap;
            startY = endY + yGap;
            endX = startX + xInc;
            endY = startY + yInc;
            ctx.beginPath();
            ctx.moveTo(startX, startY);
            ctx.lineTo(endX, endY);
            ctx.arc(endX, endY+3, 3, 1.5*Math.PI, 0.5*Math.PI, false);
            ctx.lineTo(startX, startY+6);
            ctx.arc(startX, startY+3, 3, 0.5*Math.PI, 1.5*Math.PI, false);
            ctx.closePath();
            ctx.fillStyle = ctx.strokeStyle;
            ctx.fill();
            ctx.stroke();
        }
      }
    }
  }, [tols, sport]);

  // function showPossession(home, guest) {
  useEffect(() => {
    // console.log("Updating possession indicators (%s) home: %s, guest: %s", sport, poss.home, poss.guest);
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    const noPoss = "#52525200";
    const hasPoss = "yellow";
    if (ctx) {
      ctx.lineWidth = 2;
      ctx.strokeStyle = (poss.home && sport === "Football") ? hasPoss : noPoss;
      ctx.strokeRect(254, 3, 98, 73);
      ctx.strokeStyle = (poss.guest && sport === "Football") ? hasPoss : noPoss;
      ctx.strokeRect(615, 3, 98, 73);
    }
  }, [poss, sport]);

  useEffect(() => {
    // console.log("Clear DnD panel? hasDnd %s", hasDnD);
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      // ctx.clearRect(858, 2, 220, 75);
      if (!hasDnD) {
        ctx.clearRect(849, 2, 9, 75);
      }
      else {
        ctx.fillStyle = "#30303080";
        ctx.fillRect(849, 2, 9, 75);
      }
    }
  }, [hasDnD]);

  // function downAndDistance(down, distance) {
  useEffect(() => {
    // If there is data in the Down, draw the background and write the data
    // console.log("Down: %s, Dist: %d", ballInfo.down, ballInfo.distance);
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.clearRect(858, 2, 220, 75);
      // Down-Distance BG - Only fill if 'Dn' != "   "
      if (ballInfo.down && ballInfo.down.length > 0) {
          ctx.fillStyle = "#2f2f2f99";
          ctx.fillRect(858, 2, 220, 75);
          ctx.font = "40px Segoe UI Semibold";
          ctx.fillStyle = "yellow";
          var distTxt = (ballInfo.distance > 0) ? ' & ' + ballInfo.distance : ' DOWN';
          var dndTxt = ballInfo.down + distTxt;
          ctx.fillText(dndTxt, 968 - (ctx.measureText(dndTxt).width/2), 54);
      }
    }
  }, [ballInfo.down, ballInfo.distance]);

  // function throwFlag(flag) {
  useEffect(() => {
    var c = flagCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.clearRect(0,0,135,25);
      if (flag) {
          ctx.fillStyle = "yellow";
          ctx.fillRect(0, 0, 135, 25);
          ctx.font = "italic 16pt Segoe UI Black";
          ctx.fillStyle = "black";
          ctx.fillText("FLAG", 67 - (ctx.measureText("FLAG").width/2), 21);
      }
    }
  }, [flag]);

  // function showTimeOut(home, away) {
  useEffect(() => {
    var x = (timeOuts.home) ? 0 : 361;
    var c = toCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.clearRect(0,0,461,25);
      ctx.fillStyle = "yellow";
      if (timeOuts.home || timeOuts.guest) {
          ctx.fillRect(x, 0, 100, 25);
          ctx.font = "italic 15pt Segoe UI Semibold";
          ctx.fillStyle = "black";
          ctx.fillText("TIME OUT", x + 50 - (ctx.measureText("TIME OUT").width/2), 22);
      }
    }
  }, [timeOuts]);

  var homeIcon = useMemo(() => new Image(), []);
  var guestIcon = useMemo(() => new Image(), []);

  function drawHomeIcon() {
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    var x = (homeIcon.width > homeIcon.height) ? ((homeIcon.width - homeIcon.height)/2) : 0;
    var y = 0;
    var xClip = (homeIcon.height < homeIcon.width) ? homeIcon.height : homeIcon.width;
    var yClip = homeIcon.height;
    if (ctx)
      ctx.drawImage(homeIcon, x, y, xClip, yClip, 32 - (35 * (xClip/yClip)), 1, 78, 78);
  }

  function drawGuestIcon() {
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    var x = (guestIcon.width > guestIcon.height) ? ((guestIcon.width - guestIcon.height)/2) : 0;
    var y = 0;
    var xClip = (guestIcon.height < guestIcon.width) ? guestIcon.height : guestIcon.width;
    var yClip = guestIcon.height;
    if (ctx)
      ctx.drawImage(guestIcon, x, y, xClip, yClip, 393 - (35 * (xClip/yClip)), 1, 78, 78);
  }

  homeIcon.onload = function() {
    drawHomeIcon();
  };
  guestIcon.onload = function() {
    drawGuestIcon();
  };

  useEffect(() => {
    var c = scoreCanvasRef.current;
    console.log("Updating records panel");
    var ctx = c.getContext("2d");
    if (ctx) {
      // TODO: Limit color area to just the top record panel
      ctx.clearRect(30, 2, 223, 75);
      ctx.clearRect(391, 2, 223, 75);
      ctx.fillStyle = "#30303080";
      ctx.fillRect(30, 2, 223, 75); // Main background
      ctx.fillRect(391, 2, 223, 75);
      ctx.fillStyle = colors.home;
      ctx.fillRect(30, 2, 223, 75);
      ctx.fillStyle = colors.guest;
      ctx.fillRect(391, 2, 223, 75);
      ctx.font = "16px Segoe UI Black";
      ctx.fillStyle = "white";
      if (records.home && records.home !== "") {
          ctx.fillText(records.home, 237 - ctx.measureText(records.home).width, 21);
      }
      if (records.guest && records.guest !== "") {
          ctx.fillText(records.guest, 598 - ctx.measureText(records.guest).width, 21);
      }
    }
  }, [colors, records]);

  function setupGame(ctx: CanvasRenderingContext2D) {
    // Read the game configuration and draw the game static data
    // const queryStr = window.location.search;
    // queryParms = parseQuery(queryStr);
    // console.log("URL: %s", window.location.search.substring(1));
    var gameId = (window.location.search) ? window.location.search.substring(1) : "1";
    fetch('./rest/db/game/' + gameId)
    .then(function (response) {
        return response.json();
    })
    .then(function (data) {
        // console.log("Drawing game backgrounds");
        homeIcon.src = '/Images/' + data["HomeIcon"];
        guestIcon.src = '/Images/' + data["GuestIcon"];
        setColors({home: data["HomeColor"] + "80", guest: data["GuestColor"] + "80"});
        setRecords({home: data["HomeRecord"], guest: data["GuestRecord"]});
        setNames({home: data["HomeName"].toUpperCase(), guest: data["GuestName"].toUpperCase()});
    })
    .catch(function (err) {
        console.log('error: ' + err);
    });
  }

  // function writeNames() {
  useEffect(() => {
    // Write the team names on top of logos
    // console.log("Writing team names");
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.font = "34px Segoe UI Semibold";
      ctx.fillStyle = "white";
      ctx.fillText(names.home, 237 - ctx.measureText(names.home).width, 50);
      ctx.fillText(names.guest, 598 - ctx.measureText(names.guest).width, 50);
    }
  }, [names, homeIcon, guestIcon]);

  useEffect(() => {
    var rootDiv = document.getElementById('root');
    if (rootDiv) {
      rootDiv.style.backgroundColor = '#00000000';
      document.body.style.backgroundColor = '#00000000';
    }
    const canvas = scoreCanvasRef.current;
    const source = new EventSource('/events');
    if (canvas) {
      const context = canvas.getContext('2d');
      if (context) {
        draw(context);
      }
    }
    console.log("Configuring EventSource");
    // var source = new EventSource('/events');
    source.addEventListener('open', function(e) {
      console.log("Events Connected");
    }, false);
    source.addEventListener('error', function(e) {
      var evt = e as MessageEvent;
      console.log('EVT error: %s', evt.data);
    }, false);
    source.addEventListener('score', function(e) {
        var evt = e as MessageEvent;
        console.log("score update", evt.data);
        if (evt.data && evt.data.length > 50) {
          var data = JSON.parse(evt.data);
          setSport(data["Spt"]);
          // console.log("Sport is %s (%s)", sport, data["Spt"]);
          setQuarter(ordinal(data["Pd"]));
          setScores({home: data["Hs"], guest: data["Gs"]});
          setTols({home: data["Htol"], guest: data["Gtol"]});
          setTimeOuts({home: data["Hto"], guest: data["Gto"]});
          setBallInfo({down: data["Dn"], distance: data["Dt"], ballon: data["BO"]});
          setHasDnD((data["Dn"] && data["Dn"].length > 0) ? true : false);
          setFlag(data["Fl"]);
          setPoss({home: data["Hpo"], guest: data["Gpo"]});
          if (data["Hf"] || data["Gf"]){
            setFoulInfo({fouls: {home: data["Hf"], guest: data["Gf"]},
                          bonus: {home: data["Hb"], guest: data["Gb"]}});
          }
        }
    }, false);
    source.addEventListener('clock', function(e) {
      var evt = e as MessageEvent;
      // console.log("clock update", evt.data);
      if (evt.data && evt.data.length > 15) {
        var clockJson = JSON.parse(evt.data);
        if (clockJson["Clock"]) {
          var periodLoc = clockJson["Clock"].lastIndexOf(".");
          var resultStr = "";
          if (periodLoc > 1) {
              resultStr = clockJson["Clock"].substring(0, periodLoc).trim();
          }
          else {
              resultStr = clockJson["Clock"].trim();
          }
          setClocks({game: resultStr, timeout: clockJson["ToC"], play: (clockJson["Pck"])?clockJson["Pck"]:""});
        }
      }
    }, false);
    return () => {
      if (source.OPEN) {
        console.log("Closing SSE events");
        source.close();
      }
    };
  }, []);

  return (
    <div className={mystyles.scoreBug}>
      <Helmet>
        <meta name="viewport" content="width=1920 height=1080" />
      </Helmet>
      <div className={mystyles.timeoutCanvas}>
        <canvas ref={toCanvasRef} id="timeoutCanvas" width="461" height="25" ></canvas>
      </div>
      <div className={mystyles.flagCanvas}>
        <canvas ref={flagCanvasRef} id="flagCanvas" width="135" height="25"></canvas>
      </div>
      <div className={mystyles.scoreCanvas}>
        <canvas ref={scoreCanvasRef} id="scoreCanvas" width="1200" height="80"></canvas>
      </div>
    </div>
  );
};

export default ScoreCanvas;
