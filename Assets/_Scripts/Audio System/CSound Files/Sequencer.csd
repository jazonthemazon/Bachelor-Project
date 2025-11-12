<Cabbage>

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

instr Sequencer
    kSequence[] fillarray 1, 1, 1, 1, 1, 1, 1, 1
    kBeat init 0
    
    kTempo chnget "tempo"
    kPitch chnget "pitch"
    
    if metro(kTempo) == 1 then
        if kSequence[kBeat] == 1 then
            event "i", "Synth", 0, 5, kPitch
        endif
        chnset kBeat, "beat"
        kBeat = (kBeat + 1) % 8
    endif
    
    kUpdateIndex chnget "updateSequence"
    
    if changed(kUpdateIndex) == 1 then
        kSequence[kUpdateIndex] = kSequence[kUpdateIndex] == 1 ? 0 : 1
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