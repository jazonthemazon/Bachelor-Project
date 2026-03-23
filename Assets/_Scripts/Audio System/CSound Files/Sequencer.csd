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
    
    ;probabilities
    kProb0 chnget "prob0"
    kProb1 chnget "prob1"
    kProb2 chnget "prob2"
    kProb3 chnget "prob3"
    kProb4 chnget "prob4"
    kProb5 chnget "prob5"
    kProb6 chnget "prob6"
    kProb7 chnget "prob7"
    
    ;speeds
    kSpeed0 chnget "speed0"
    kSpeed1 chnget "speed1"
    kSpeed2 chnget "speed2"
    kSpeed3 chnget "speed3"
    kSpeed4 chnget "speed4"
    kSpeed5 chnget "speed5"
    kSpeed6 chnget "speed6"
    kSpeed7 chnget "speed7"
    
    ;instruments
    kInstrument0 chnget "instrument0"
    kInstrument1 chnget "instrument1"
    kInstrument2 chnget "instrument2"
    kInstrument3 chnget "instrument3"
    kInstrument4 chnget "instrument4"
    kInstrument5 chnget "instrument5"
    kInstrument6 chnget "instrument6"
    kInstrument7 chnget "instrument7"
    
    ;pitches
    kPitch0 chnget "pitch0"
    kPitch1 chnget "pitch1"
    kPitch2 chnget "pitch2"
    kPitch3 chnget "pitch3"
    kPitch4 chnget "pitch4"
    kPitch5 chnget "pitch5"
    kPitch6 chnget "pitch6"
    kPitch7 chnget "pitch7"
    
    ;note lengths
    kLength0 chnget "length0"
    kLength1 chnget "length1"
    kLength2 chnget "length2"
    kLength3 chnget "length3"
    kLength4 chnget "length4"
    kLength5 chnget "length5"
    kLength6 chnget "length6"
    kLength7 chnget "length7"
    
    ;volumes
    kVolume0 chnget "volume0"
    kVolume1 chnget "volume1"
    kVolume2 chnget "volume2"
    kVolume3 chnget "volume3"
    kVolume4 chnget "volume4"
    kVolume5 chnget "volume5"
    kVolume6 chnget "volume6"
    kVolume7 chnget "volume7"
    
    if metro(kTempo) == 1 then
        
        kRand0 random 0, 1
        
        if kActive0 == 1 && kBeat % kSpeed0 == 0 && kRand0 < kProb0 then
            event "i", kInstrument0, 0, kLength0, kPitch0, kVolume0
        endif
        
        kRand1 random 0, 1
        
        if kActive1 == 1 && kBeat % kSpeed1 == 0 && kRand1 < kProb1 then
            event "i", kInstrument1, 0, kLength1, kPitch1, kVolume1
        endif
        
        kRand2 random 0, 1
        
        if kActive2 == 1 && kBeat % kSpeed2 == 0 && kRand2 < kProb2 then
            event "i", kInstrument2, 0, kLength2, kPitch2, kVolume2
        endif
        
        kRand3 random 0, 1
        
        if kActive3 == 1 && kBeat % kSpeed3 == 0 && kRand3 < kProb3 then
            event "i", kInstrument3, 0, kLength3, kPitch3, kVolume3
        endif
        
        kRand4 random 0, 1
        
        if kActive4 == 1 && kBeat % kSpeed4 == 0 && kRand4 < kProb4 then
            event "i", kInstrument4, 0, kLength4, kPitch4, kVolume4
        endif
        
        kRand5 random 0, 1
        
        if kActive5 == 1 && kBeat % kSpeed5 == 0 && kRand5 < kProb5 then
            event "i", kInstrument5, 0, kLength5, kPitch5, kVolume5
        endif
        
        kRand6 random 0, 1
        
        if kActive6 == 1 && kBeat % kSpeed6 == 0 && kRand6 < kProb6 then
            event "i", kInstrument6, 0, kLength6, kPitch6, kVolume6
        endif
        
        kRand7 random 0, 1
        
        if kActive7 == 1 && kBeat % kSpeed7 == 0 && kRand7 < kProb7 then
            event "i", kInstrument7, 0, kLength7, kPitch7, kVolume7
        endif
        
        kBeat = kBeat + 1
    endif
endin

instr Synth

    a1 expon .1, p3, 0.0001
    aOut oscili a1, p4
    outs aOut * p5, aOut * p5
    
endin

instr Piano

    ;has to be balanced more for better sound
    aOutLeft,aOutRight prepiano p4, 2, 0, 1, p3 / 3, 0.002, 2, 2, 1, 5000, -0.01, 0.09, 80, 0, 0.1, 1, 2
    
    outs aOutLeft * p5, aOutRight * p5
    
endin

instr Guitar

    ;has to be balanced more for better sound / still clicking sounds on end of notes - use wgpluck2 instead
    asig pluck p5, p4, p4, 0, 1, .5, 10
    
    outs asig, asig
    
endin

instr Bass

    iplk = .2
    kamp = 1
    icps = p4
    kpick = 0.2
    krefl = .8

    apluck wgpluck2 iplk, kamp, icps, kpick, krefl
    apluck dcblock2 apluck
    outs apluck * p5, apluck * p5

endin

instr 11

idur  = p3 ; Duration
iamp  = p4 ; Amplitude
iacc  = p5 ; Accent
irez  = p6 ; Resonance
iod   = p7 ; Overdrive
ilowf = p8 ; Low Frequency

kfenv  linseg    1000*iacc,  .02, 180, .04, 120, idur-.06, ilowf ; Freq Envelope
kaenv  expseg    .1, .001, 1, .02, 1, .04, .7, idur-.062, .7  ; Amp Envelope
kdclck linseg    0, .002, 1, idur-.042, 1, .04, 0             ; Declick
asig   rand      2 ; Random number

aflt   rezzy     asig, kfenv, irez*40         ; Filter

aout1  =         aflt*kaenv*3*iod/iacc        ; Scale the sound

krms   rms       aout1, 1000                  ; Limiter, get rms
klim   table3    krms*.5, 5, 1                ; Get limiting value
aout   =         aout1*klim*iamp*kdclck/sqrt(iod)*1.3   ; Scale again and ouput

       outs      aout, aout                   ; Output the sound

       endin

</CsInstruments>
<CsScore>
f1 0 8 2 1 0.6 10 100 0.001 ;; 1 rattle
f2 0 8 2 1 0.7 50 500 1000  ;; 1 rubber

i "Sequencer" 0 [3600 * 12]
</CsScore>
</CsoundSynthesizer>