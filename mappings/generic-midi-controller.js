// Generic MIDI Controller Template for MIXERX
// Customize this template for any MIDI controller
// Use MIDI monitoring tools to find your controller's CC/Note numbers

function onMidiMessage(msg) {
    const channel = msg.channel;
    const control = msg.data1;
    const value = msg.data2;
    
    console.log(`MIDI: Ch${channel} Control${control} Value${value}`);
    
    // === DECK SELECTION ===
    // Customize based on your controller layout
    let deckId = 1;
    if (channel === 1) deckId = 2;
    if (channel === 2) deckId = 3;
    if (channel === 3) deckId = 4;
    
    const deck = getDeck(deckId);
    
    // === TRANSPORT CONTROLS ===
    // Customize these note numbers for your controller
    
    // Play/Pause Button
    if (msg.isNoteOn(0x20)) { // Change 0x20 to your play button note
        deck.playPause();
    }
    
    // Cue Button
    if (msg.isNoteOn(0x21)) { // Change 0x21 to your cue button note
        deck.cue();
    }
    
    // Sync Button
    if (msg.isNoteOn(0x22)) { // Change 0x22 to your sync button note
        deck.sync();
    }
    
    // === FADERS & KNOBS ===
    // Customize these CC numbers for your controller
    
    // Tempo Fader/Knob
    if (msg.isCC(0x01)) { // Change 0x01 to your tempo control CC
        const tempo = 0.5 + (value / 127.0) * 1.5; // 50% to 200% speed
        deck.setTempo(tempo);
    }
    
    // Volume Fader
    if (msg.isCC(0x07)) { // Change 0x07 to your volume fader CC
        deck.setVolume(value / 127.0);
    }
    
    // === EQ CONTROLS ===
    
    // High EQ
    if (msg.isCC(0x10)) { // Change 0x10 to your high EQ knob CC
        deck.setParameter("eq.high", value / 64.0); // 0-2x gain
    }
    
    // Mid EQ
    if (msg.isCC(0x11)) { // Change 0x11 to your mid EQ knob CC
        deck.setParameter("eq.mid", value / 64.0);
    }
    
    // Low EQ
    if (msg.isCC(0x12)) { // Change 0x12 to your low EQ knob CC
        deck.setParameter("eq.low", value / 64.0);
    }
    
    // === FILTER ===
    
    // Filter Cutoff
    if (msg.isCC(0x20)) { // Change 0x20 to your filter knob CC
        deck.setParameter("filter.cutoff", value / 127.0);
    }
    
    // Filter Resonance
    if (msg.isCC(0x21)) { // Change 0x21 to your resonance knob CC
        deck.setParameter("filter.resonance", value / 127.0);
    }
    
    // === CROSSFADER ===
    
    if (msg.isCC(0x0F)) { // Change 0x0F to your crossfader CC
        const position = (value - 64) / 64.0; // -1 to +1
        handleCrossfader(position);
    }
}

function handleCrossfader(position) {
    // Simple A/B crossfader
    const leftGain = Math.max(0, 1 - Math.max(0, position));
    const rightGain = Math.max(0, 1 + Math.min(0, position));
    
    getDeck(1).setVolume(leftGain);
    getDeck(2).setVolume(rightGain);
}

// === CUSTOMIZATION GUIDE ===
/*
1. Connect your MIDI controller
2. Use MIDI monitoring software to find control numbers
3. Replace the CC/Note numbers in this template
4. Test each control in MIXERX
5. Save as "your-controller-name.js"

Common MIDI CC Numbers:
- Volume Faders: CC 7
- Pan: CC 10  
- Expression: CC 11
- Modulation: CC 1
- Sustain Pedal: CC 64

Common Note Numbers:
- C4 = 60, C#4 = 61, D4 = 62, etc.
- Pads often start at 36 (C2)
*/
