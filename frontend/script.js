const Diagnostics = require("Diagnostics")
Diagnostics.log("Running script.js")

const Scene = require('Scene');
const Networking = require('Networking');
const Time = require('Time');
var Textures = require('Textures');
var Materials = require('Materials');
var Patches = require('Patches');
var Reactive = require('Reactive')

var reverse = false;

var texs = [Textures.get('current_texture'), Textures.get('next_texture')];
var preload_time = 1;
var url = "https://htest.eshc.coop/api/city/heatmap/" + preload_time + ".jpg";
texs[1].url = url

Time.setInterval(() => {
    if (reverse) {
        Patches.setPulseValue("triggerBlendAnimationReverse", Reactive.once());
        reverse = false;
    } else {
        Patches.setPulseValue("triggerBlendAnimation", Reactive.once());
        reverse = true;
    }
    Patches.getPulseValue('animationCompleted').subscribe(function (e) {
        texs = texs.reverse();
        preload_time += 1;
        if (preload_time == 24) {
            preload_time = 0;
        }
        url = "https://htest.eshc.coop/api/city/heatmap/" + preload_time + ".jpg";
        texs[1].url = url
    })
}, 5000);
