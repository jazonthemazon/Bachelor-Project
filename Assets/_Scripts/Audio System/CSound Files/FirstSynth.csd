<Cabbage>
form caption("SimpleFreq")
hslider channel("freq1"), text("Frequency Slider"), range(0, 10000, 0)
button channel("trigger"), text("Push me")
checkbox channel("mute") bounds(16, 80, 28, 30) colour:0(101, 130, 101, 97)
checkbox channel("mute")

</Cabbage>
<CsoundSynthesizer>
<CsOptions>
; Select audio/midi flags here according to platform
-odac     ;;;realtime audio out
;-iadc    ;;;uncomment -iadc if RT audio input is needed too
; For Non-realtime ouput leave only the line below:
; -o t.wav -W ;;; for file output any platform
</CsOptions>
<CsInstruments>

sr = 48000
ksmps = 32
nchnls = 2
0dbfs  = 1

instr 1
    kEnv madsr .2, .2, .6, .4
    aOut vco2 p5, p4
    outs aOut*kEnv, aOut*kEnv
endin

</CsInstruments>
<CsScore>
;causes Csound to run for about 7000 years...
f0 z

t 0 2 12 30 24 240
</CsScore>
</CsoundSynthesizer>