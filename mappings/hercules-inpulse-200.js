// Hercules DJControl Inpulse 200 Mapping for MIXERX
// Entry-level controller with essential DJ controls

function onMidiMessage(msg) {
    const channel = msg.channel;
    const note = msg.data1;
    const value = msg.data2;
    
    // Deck mapping: Channel 0 = Deck 1, Channel 1 = Deck 2
    const deckId = channel + 1;
    const deck = getDeck(deckId);
    
    // Play/Pause (Note On)
    if (msg.isNoteOn(0x03)) {
        deck.playPause();
        // LED feedback
        sendMidi(0x90 + channel, 0x03, deck.isPlaying ? 127 : 0);
    }
    
    // Cue Button
    if (msg.isNoteOn(0x01)) {
        deck.cue();
        sendMidi(0x90 + channel, 0x01, value > 0 ? 127 : 0);
    }
    
    // Sync Button
    if (msg.isNoteOn(0x02)) {
        deck.sync();
        sendMidi(0x90 + channel, 0x02, value > 0 ? 127 : 0);
    }
    
    // Tempo Slider
    if (msg.isCC(0x01)) {
        const tempo = 0.8 + (value / 127.0) * 0.4; // ±20% tempo range
        deck.setTempo(tempo);
    }
    
    // EQ Knobs
    if (msg.isCC(0x07)) { // Treble
        deck.setParameter("eq.high", value / 64.0);
    }
    if (msg.isCC(0x08)) { // Medium  
        deck.setParameter("eq.mid", value / 64.0);
    }
    if (msg.isCC(0x09)) { // Bass
        deck.setParameter("eq.low", value / 64.0);
    }
    
    // Volume Fader
    if (msg.isCC(0x0A)) {
        deck.setVolume(value / 127.0);
    }
    
    // Gain Knob
    if (msg.isCC(0x0B)) {
        const gain = 0.5 + (value / 127.0) * 1.5; // 0.5x to 2.0x
        deck.setParameter("gain", gain);
    }
}

// Auto-detect controller
function onControllerConnect() {
    console.log("Hercules DJControl Inpulse 200 connected");
    
    // Initialize LED states
    for (let channel = 0; channel < 2; channel++) {
        sendMidi(0x90 + channel, 0x03, 0); // Play LED off
        sendMidi(0x90 + channel, 0x01, 0); // Cue LED off
        sendMidi(0x90 + channel, 0x02, 0); // Sync LED off
    }
}
