/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

window.onload = function()
{

};

function blossom(flower){

    setTimeout(() => {flower.src = "assets/logopics/flower_1.png"}, 100);
    setTimeout(() => {flower.src = "assets/logopics/flower_2.png"}, 200);
    setTimeout(() => {flower.src = "assets/logopics/flower_3.png"}, 300);
    setTimeout(() => {flower.src = "assets/img/bigwarmflower.png"}, 400);
}

function wither(flower){
    flower.src = "assets/img/bigwarmflower.png";
}