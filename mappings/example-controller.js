// Example Controller Mapping for Generic MIDI Controller
// MIXERX JavaScript Mapping API

// Called when MIDI message is received
function onMidiMessage(msg) {
    const deck1 = getDeck(1);
    const deck2 = getDeck(2);
    
    // Deck 1 Controls
    if (msg.isNoteOn(0x10)) {
        deck1.playPause();
        sendMidi(0x90, 0x10, msg.isPlaying ? 127 : 0); // LED feedback
    }
    
    if (msg.isNoteOn(0x11)) {
        deck1.cue();
    }
    
    if (msg.isNoteOn(0x12)) {
        deck1.sync();
    }
    
    // Deck 2 Controls  
    if (msg.isNoteOn(0x20)) {
        deck2.playPause();
        sendMidi(0x90, 0x20, msg.isPlaying ? 127 : 0);
    }
    
    if (msg.isNoteOn(0x21)) {
        deck2.cue();
    }
    
    if (msg.isNoteOn(0x22)) {
        deck2.sync();
    }
    
    // Tempo Controls
    if (msg.isCC(0x30)) {
        const tempo = 0.5 + (msg.value * 1.5); // 0.5x to 2.0x
        deck1.setTempo(tempo);
    }
    
    if (msg.isCC(0x31)) {
        const tempo = 0.5 + (msg.value * 1.5);
        deck2.setTempo(tempo);
    }
    
    // Position/Jog Wheels
    if (msg.isCC(0x40)) {
        const position = msg.value * 100; // 0-100%
        deck1.setPosition(position);
    }
    
    if (msg.isCC(0x41)) {
        const position = msg.value * 100;
        deck2.setPosition(position);
    }
    
    console.log(`MIDI: ${msg.status.toString(16)} ${msg.data1.toString(16)} ${msg.data2.toString(16)}`);
}

// Called when deck state changes (for LED feedback)
function onDeckStateChange(deckId, state) {
    if (deckId === 1) {
        sendMidi(0x90, 0x10, state.isPlaying ? 127 : 0);
    } else if (deckId === 2) {
        sendMidi(0x90, 0x20, state.isPlaying ? 127 : 0);
    }
}
