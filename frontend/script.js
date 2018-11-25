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

var preload_time = 1;
const getTimeURL = time => "https://htest.eshc.coop/api/city/heatmap/" + time + ".jpg";

var texs = [Textures.get('current_texture'), Textures.get('next_texture')];

texs[1].url = getTimeURL(1)

// Lol this crashy
Diagnostics.watch("animationCompleted", Patches.getScalarValue('animationCompleted'))

textNode.text = Patches.getScalarValue('animationCompleted')//.format("{02}")

Patches.getScalarValue('animationCompleted').monitor().subscribe((e, a) => {
    Diagnostics.log("Animation completed")
    Diagnostics.log(e.newValue)

    let s = e.newValue+"";
    if (e.newValue < 10) {
        s = "0" + s;
    }

    // textNode.text = s + ":00";
    texs = texs.reverse();

    texs[1].url = getTimeURL(e.newValue);
})



// const interval = () => {
//     const key = reverse ? "triggerBlendAnimationReverse" : "triggerBlendAnimation";
//     Diagnostics.log("Trigger " + key)
//     Patches.setPulseValue(key, Reactive.once());

//     reverse = !reverse;
// }

// Time.ms.interval(1750).subscribe(interval);
// Time.setTimeout(() => {
    // interval()
    // Time.setInterval(interval, 1750);
// }, 1000);
