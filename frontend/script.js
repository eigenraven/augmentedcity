const Diagnostics = require("Diagnostics")
Diagnostics.log("Running script.js")

const Scene = require('Scene');
const Networking = require('Networking');
const Time = require('Time');
var Textures = require('Textures');
var Materials = require('Materials');
var Patches = require('Patches');
var Reactive = require('Reactive')


const textNode = Scene.root.find('clockText');

var reverse = false;

var preload_time = 1;
const getTimeURL = time => "https://htest.eshc.coop/api/city/heatmap/" + time + ".jpg";

var texs = [Textures.get('current_texture'), Textures.get('next_texture')];

texs[1].url = getTimeURL(preload_time)

Patches.getPulseValue('animationCompleted').subscribe(e => {
    Diagnostics.log("Animation completed.")

    let s = preload_time+"";
    if (preload_time < 10) {
        s = "0" + s;
    }

    textNode.text = s + ":00";
    texs = texs.reverse();
    preload_time += 2;
    if (preload_time >= 24) {
        preload_time = 0;
    }
    texs[1].url = getTimeURL(preload_time);
})

const interval = () => {
    const key = reverse ? "triggerBlendAnimationReverse" : "triggerBlendAnimation";
    Diagnostics.log("Trigger " + key)
    Patches.setPulseValue(key, Reactive.once());
    reverse = !reverse;
}

// Time.setTimeout(() => {
    // interval()
    Time.setInterval(interval, 1750);
// }, 1000);
