/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

window.onload = function()
{
    fInit();
    
    document.getElementById("nextbtn").onclick = FNext;
    document.getElementById("previousbtn").onclick = FPrev;
    document.getElementById("playbtn").onclick = FAuto;
};

//Pseudo-Class JS object 
function PicFrame(displayname, imgnum, viewnum) //CTOR like Definition
{
    this.Name = displayname;
    var img = new Image();
    img.src = "images/pic_" + imgnum + ".jpg";
    this.Pic = img;
    var viewcounter = viewnum;
    this.View = viewcounter;
}

var picarr = [];   //empty array?
var piclabel = ["Go Go", "Mic Drop", "DNA", "No More Dream", "BTS"];
var currentpic = 0;
var autoflag = false;
var timerID = 0;

function fInit()
{
    for (var i = 0; i < piclabel.length; i++)
    {
        var newpicframe = new PicFrame(piclabel[i], i + 1, 0);
        picarr.push(newpicframe);
    }
    
    showPic();
}

function showPic()
{
    var currentslide = picarr[currentpic];
    currentslide.View++;
    document.getElementById("displaypic").src = currentslide.Pic.src;
    document.getElementById("picname").value = currentslide.Name + " : " + currentslide.View;
}

function FNext()
{
    currentpic++;
    
    currentpic = (currentpic + picarr.length) % picarr.length;
    
    showPic();
}

function FPrev()
{
    currentpic--;
    
    currentpic = (currentpic + picarr.length) % picarr.length;
    
    showPic();
}

function FAuto()
{
    if(autoflag === false)
    {
        autoflag = true;
        document.getElementById("playpausebtn").src = "btnimgs/pause.jpg";
        //if no timer set, then fire timer to FNext
        if (timerID > -1)
        {
            timerID = window.setInterval(FNext,500);
        }
        
    }
    else
    {
        document.getElementById("playpausebtn").src = "btnimgs/play.jpg";
        autoflag = false;
        window.clearInterval(timerID); //stop any valid timer with ID
        timerID = 0;
    }
}