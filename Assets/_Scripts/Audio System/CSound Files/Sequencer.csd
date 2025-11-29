<Cabbage>
</Cabbage>
<CsoundSynthesizer>
<CsOptions>
-odac
</CsOptions>
<CsInstruments>

sr = 48000
ksmps = 32
nchnls = 2
0dbfs  = 1

instr Sequencer
    kBeat init 0
    
    ;global parameters
    kTempo chnget "tempo"
    kSwing chnget "swing"
    kNumberOfTunedInstruments chnget "numberOfTunedInstruments"
    
    ;instrument specific parameters
    
    ;active
    kActive0 chnget "active0"
    kActive1 chnget "active1"
    kActive2 chnget "active2"
    kActive3 chnget "active3"
    kActive4 chnget "active4"
    kActive5 chnget "active5"
    kActive6 chnget "active6"
    kActive7 chnget "active7"
    
    ;speeds
    kSpeed0 chnget "speed0"
    kSpeed1 chnget "speed1"
    kSpeed2 chnget "speed2"
    kSpeed3 chnget "speed3"
    kSpeed4 chnget "speed4"
    kSpeed5 chnget "speed5"
    kSpeed6 chnget "speed6"
    kSpeed7 chnget "speed7"
    
    ;pitches
    kPitch0 chnget "pitch0"
    kPitch1 chnget "pitch1"
    kPitch2 chnget "pitch2"
    kPitch3 chnget "pitch3"
    kPitch4 chnget "pitch4"
    kPitch5 chnget "pitch5"
    kPitch6 chnget "pitch6"
    kPitch7 chnget "pitch7"
    
    ;volumes
    kVolume0 chnget "volume0"
    kVolume1 chnget "volume1"
    kVolume2 chnget "volume2"
    kVolume3 chnget "volume3"
    kVolume4 chnget "volume4"
    kVolume5 chnget "volume5"
    kVolume6 chnget "volume6"
    kVolume7 chnget "volume7"
    
    kSynthPitch chnget "synthPitch"
    kBassPitch chnget "bassPitch"
    
    kSwingDelay init 0
    
    if metro(kTempo) == 1 then
        kSwingDelay = kBeat % 2 == 0 ? 0 : (1 / kTempo) * ((kSwing - 50) / 50)
        
        if kActive0 == 1 && kBeat % kSpeed0 == 0 then
            event "i", "Synth", kSwingDelay, 5, kPitch0
        endif
        
        if kActive1 == 1 && kBeat % kSpeed1 == 0  then
            event "i", "Synth", kSwingDelay, 5, kPitch1
        endif
        
        if kActive2 == 1 && kBeat % kSpeed2 == 0  then
            event "i", "Synth", kSwingDelay, 5, kPitch2
        endif
        
        if kActive3 == 1 && kBeat % kSpeed3 == 0  then
            event "i", "Synth", kSwingDelay, 5, kPitch3
        endif
        
        if kActive4 == 1 && kBeat % kSpeed4 == 0  then
            event "i", "Synth", kSwingDelay, 5, kPitch4
        endif
        
        if kActive5 == 1 && kBeat % kSpeed5 == 0  then
            event "i", "Synth", kSwingDelay, 5, kPitch5
        endif
        
        if kActive6 == 1 && kBeat % kSpeed6 == 0  then
            event "i", "Synth", kSwingDelay, 5, kPitch6
        endif
        
        if kActive7 == 1 && kBeat % kSpeed7 == 0  then
            event "i", "Synth", kSwingDelay, 5, kPitch7
        endif
        
        kBeat = kBeat + 1
    endif
endin

instr Synth
    a1 expon .1, p3, 0.0001
    aOut oscili a1, p4
    outs aOut, aOut
endin

</CsInstruments>
<CsScore>
i "Sequencer" 0 [3600 * 12]
</CsScore>
</CsoundSynthesizer>