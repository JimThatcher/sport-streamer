import React, { useRef, useEffect, useState, useMemo } from 'react';

const ScoreCanvas = () => {

  // const canvasRef = useRef() as React.MutableRefObject<HTMLCanvasElement>;
  const scoreCanvasRef = useRef() as React.MutableRefObject<HTMLCanvasElement>;
  const [homeName, setHomeName] = useState<string>("");
  const [guestName, setGuestName] = useState<string>("");
  const [homeScore, setHomeScore] = useState<number>(-1);
  const [guestScore, setGuestScore] = useState<number>(-1);
  const [gameClock, setGameClock] = useState<string>("-:--");
  const [quarter, setQuarter] = useState<string>("");
  const [homeTOL, setHomeTOL] = useState<number>(0);
  const [guestTOL, setGuestTOL] = useState<number>(0);
  const [homePoss, setHomePoss] = useState<boolean>(false);
  const [guestPoss, setGuestPoss] = useState<boolean>(false);
  const [down, setDown] = useState<string>("");
  const [distance, setDistance] = useState<number>(0);
  const [hasDnD, setHasDnd] = useState<boolean>(false);

  function ordinal(n: number) {
    var s = ["th", "st", "nd", "rd"];
    var v = n%100;
    return n + (s[(v-20)%10] || s[v] || s[0]);
  }

  const draw = (ctx: CanvasRenderingContext2D) => {
    console.log("inside of draw()");
    setupScoreboard(ctx);
    setupGame(ctx);
  };

  function setupScoreboard(ctx: CanvasRenderingContext2D) {
    // Draw the basic background for the scoreboard
    if (ctx) {
      ctx.fillStyle = "#30303080";
      ctx.fillRect(30, 2, 821, 75); // Main background
      ctx.fillStyle = "#525252cc";
      ctx.fillRect(253, 2, 100, 75); // Home score background
      ctx.fillRect(614, 2, 100, 75); // guest score background
      ctx.fillStyle = "#282828cc";
      ctx.fillRect(731, 2, 120, 75); // Period/Time background
    }
  }

  // function scores(homeScore, guestScore) {
  useEffect(() => {
    console.log("Score update - home: %d, guest: %d", homeScore, guestScore);
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
      ctx.fillText(homeScore.toString(), 303 - (ctx.measureText(homeScore.toString()).width/2), 65);
      ctx.fillText(guestScore.toString(), 662 - (ctx.measureText(guestScore.toString()).width/2), 65);
    }
  }, [homeScore, guestScore]);

  // function quarter(quarter) {
  useEffect(() => {
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.clearRect(731, 2, 120, 38);
      ctx.fillStyle = "#282828cc";
      ctx.fillRect(731, 2, 120, 38); // Period/Time background
      ctx.font = "24pt Segoe UI Semibold";
      ctx.fillStyle = "yellow";
      ctx.fillText(quarter, 791 - (ctx.measureText(quarter).width/2), 35);
    }
  }, [quarter]);

  // function clock(clock) {
  useEffect(() => {
    console.log("Clock update: %s", gameClock);
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.clearRect(731, 39, 120, 38);
      ctx.fillStyle = "#282828cc";
      ctx.fillRect(731, 39, 120, 38); // Period/Time background
      ctx.font = "24pt Segoe UI Semibold";
      if (gameClock !== "-:--") {
          ctx.fillStyle = "white";
          ctx.fillText(gameClock, 791 - (ctx.measureText(gameClock).width/2), 70);
      }
    }
  }, [gameClock]);

  // function timeOuts(home, guest) {
  useEffect(() => {
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    var noTO = "#525252ff";
    var hasTO = "#ffff00ff";
    if (ctx) {
      ctx.lineWidth = 8;
      ctx.lineCap = "round";
      var start = 0;
      var end = 145;
      for (let i=1; i<=3; i++) {
          ctx.strokeStyle = (homeTOL >= i) ? hasTO : noTO;
          start = end + 15;
          end = start + 10;
          ctx.beginPath();
          ctx.moveTo(start, 63);
          ctx.lineTo(end, 63);
          ctx.stroke();
      }
      end = 506;
      for (let i=1; i<=3; i++) {
          ctx.strokeStyle = (guestTOL >= i) ? hasTO : noTO;
          start = end + 15;
          end = start + 10;
          ctx.beginPath();
          ctx.moveTo(start, 63);
          ctx.lineTo(end, 63);
          ctx.stroke();
      }
    }
  }, [homeTOL, guestTOL]);

  // function showPossession(home, guest) {
  useEffect(() => {
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    const noPoss = "#52525200";
    const hasPoss = "yellow";
    if (ctx) {
      ctx.lineWidth = 2;
      ctx.strokeStyle = (homePoss) ? hasPoss : noPoss;
      ctx.strokeRect(254, 3, 98, 73);
      ctx.strokeStyle = (guestPoss) ? hasPoss : noPoss;
      ctx.strokeRect(615, 3, 98, 73);
    }
  }, [homePoss, guestPoss]);

  // function downAndDistance(down, distance) {
  useEffect(() => {
    // If there is data in the Down, draw the background and write the data
    console.log("Down: %s, Dist: %d", down, distance);
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.clearRect(858, 2, 220, 75);
      // Down-Distance BG - Only fill if 'Dn' != "   "
      if (down && down.length > 0) {
          if (!hasDnD) {
              setHasDnd(true);
              ctx.fillStyle = "#30303080";
              ctx.fillRect(851, 2, 7, 75);
          }
          ctx.fillStyle = "#2f2f2f99";
          ctx.fillRect(858, 2, 220, 75);
          ctx.font = "40px Segoe UI Semibold";
          ctx.fillStyle = "yellow";
          var distTxt = (distance > 0) ? ' & ' + distance : ' DOWN';
          var dndTxt = down + distTxt;
          ctx.fillText(dndTxt, 968 - (ctx.measureText(dndTxt).width/2), 54);
      }
      if (down && down.length <= 1 && hasDnD)
      {
          setHasDnd(false);
          ctx.clearRect(851, 2, 7, 75);
      }
    }
  }, [down, distance, hasDnD]);


  var homeIcon = useMemo(() => new Image(), []);
  var guestIcon = useMemo(() => new Image(), []);
  // var hasDnD = false;
  // var homeName = "";
  // var guestName = "";
  // var homeTO = false;
  // var guestTO = false;
  // var flagState = false;

  homeIcon.onload = function() {
      var c = scoreCanvasRef.current;
      var ctx = c.getContext("2d");
      var x = (homeIcon.width > homeIcon.height) ? ((homeIcon.width - homeIcon.height)/2) : 0;
      var y = 0;
      var xClip = (homeIcon.height < homeIcon.width) ? homeIcon.height : homeIcon.width;
      var yClip = homeIcon.height;
      if (ctx)
        ctx.drawImage(homeIcon, x, y, xClip, yClip, 32 - (35 * (xClip/yClip)), 1, 78, 78);
  };
  guestIcon.onload = function() {
      var c = scoreCanvasRef.current;
      var ctx = c.getContext("2d");
      var x = (guestIcon.width > guestIcon.height) ? ((guestIcon.width - guestIcon.height)/2) : 0;
      var y = 0;
      var xClip = (guestIcon.height < guestIcon.width) ? guestIcon.height : guestIcon.width;
      var yClip = guestIcon.height;
      if (ctx)
        ctx.drawImage(guestIcon, x, y, xClip, yClip, 393 - (35 * (xClip/yClip)), 1, 78, 78);
      // writeNames();
  };

  function setupGame(ctx: CanvasRenderingContext2D) {
    // Read the game configuration and draw the game static data
    // const queryStr = window.location.search;
    // queryParms = parseQuery(queryStr);
    fetch('./rest/db/game/' + String(1))
    .then(function (response) {
        return response.json();
    })
    .then(function (data) {
        console.log("Drawing game backgrounds");
        homeIcon.src = '/Images/' + data["HomeIcon"];
        guestIcon.src = '/Images/' + data["GuestIcon"];
        // var c = scoreCanvasRef.current;
        // var ctx = c.getContext("2d");
        if (ctx) {
          ctx.fillStyle = data["HomeColor"];
          ctx.fillRect(30, 2, 223, 75);
          ctx.fillStyle = data["GuestColor"];
          ctx.fillRect(391, 2, 223, 75);
          ctx.font = "16px Segoe UI Black";
          ctx.fillStyle = "white";
          if (data["HomeRecord"] && data["HomeRecord"] !== "") {
              ctx.fillText(data["HomeRecord"], 110, 68);
          }
          if (data["GuestRecord"] && data["GuestRecord"] !== "") {
              ctx.fillText(data["GuestRecord"], 471, 68);
          }
        }
        setHomeName(data["HomeName"].toUpperCase());
        setGuestName(data["GuestName"].toUpperCase());
    })
    .catch(function (err) {
        console.log('error: ' + err);
    });
  }

  // function writeNames() {
  useEffect(() => {
    // Write the team names on top of logos
    console.log("Writing team names");
    var c = scoreCanvasRef.current;
    var ctx = c.getContext("2d");
    if (ctx) {
      ctx.font = "36px Segoe UI Semibold";
      ctx.fillStyle = "white";
      ctx.fillText(homeName, 247 - ctx.measureText(homeName).width, 50);
      ctx.fillText(guestName, 608 - ctx.measureText(guestName).width, 50);
    }
  }, [homeName, guestName, homeIcon, guestIcon]);

  useEffect(() => {
    const canvas = scoreCanvasRef.current;
    const source = new EventSource('/events');
    console.log("Getting Base canvas context");
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
          setDown(data["Dn"]);
          setDistance(data["Dt"]);
          setQuarter(ordinal(data["Pd"]));
          setHomeScore(data["Hs"]);
          setGuestScore(data["Gs"]);
          setHomeTOL(data["Htol"]);
          setGuestTOL(data["Gtol"]);
          setHomePoss(data["Hpo"]);
          setGuestPoss(data["Gpo"]);
        }
        /* if (data["Fl"] !== flagState) {
            flagState = data["Fl"];
            // throwFlag(data["Fl"]);
        }
        if (data["Hto"] !== homeTO || data["Gto"] !== guestTO) {
            homeTO = (data["Hto"]);
            guestTO = (data["Gto"]);
            // showTimeOut(homeTO, guestTO);
        } */
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
          setGameClock(resultStr);
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
  /*
    const content = () => {
      <>
      <div style={{width: 1200, height: 80}}>
        <canvas ref={scoreCanvasRef} id="scoreCanvas" width="1200" height="80"></canvas>
      </div>
      </>
    };
  */
  return (
    <div style={{width: 1200, height: 80}}>
      <canvas ref={scoreCanvasRef} id="scoreCanvas" width="1200" height="80"></canvas>
    </div>
  );
};

export default ScoreCanvas;
