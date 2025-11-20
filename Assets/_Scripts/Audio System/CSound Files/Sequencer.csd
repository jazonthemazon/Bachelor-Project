<Cabbage>
</Cabbage>
<CsoundSynthesizer>
<CsOptions>
-odac
--env:SSDIR+=../../../Audio/Samples
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
    kSwing chnget "swing"
    kSwingDelay init 0
    
    if metro(kTempo) == 1 then
        kSwingDelay = kBeat % 2 == 0 ? 0 : (1 / kTempo) * ((kSwing - 50) / 50)
        
        if kSequence[kBeat] == 1 then
            event "i", "Synth", kSwingDelay, 5, kPitch
        endif
        kBeat = (kBeat + 1) % 8
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