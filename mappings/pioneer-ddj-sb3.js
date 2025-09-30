// Pioneer DDJ-SB3 Controller Mapping for MIXERX
// Supports: Play/Pause, Tempo, EQ, Filter, Sync, Cue

function onMidiMessage(msg) {
    const channel = msg.channel;
    const note = msg.data1;
    const value = msg.data2;
    
    // Deck mapping: Channel 0-1 = Deck 1-2, Channel 2-3 = Deck 3-4
    const deckId = Math.floor(channel / 2) + 1;
    const deck = getDeck(deckId);
    
    // Play/Pause Buttons
    if (msg.isNoteOn(0x0B)) { // Play button
        deck.playPause();
        sendMidi(0x90 + channel, 0x0B, value > 0 ? 127 : 0); // LED feedback
    }
    
    // Cue Button
    if (msg.isNoteOn(0x0C)) { // Cue button
        deck.cue();
        sendMidi(0x90 + channel, 0x0C, value > 0 ? 127 : 0);
    }
    
    // Sync Button
    if (msg.isNoteOn(0x58)) { // Sync button
        deck.sync();
        sendMidi(0x90 + channel, 0x58, value > 0 ? 127 : 0);
    }
    
    // Tempo Fader
    if (msg.isCC(0x00)) { // Tempo fader
        const tempo = 0.5 + (value / 127.0) * 1.5; // 0.5x to 2.0x
        deck.setTempo(tempo);
    }
    
    // EQ Controls
    if (msg.isCC(0x07)) { // EQ High
        const gain = value / 64.0; // 0-2x gain
        deck.setParameter("eq.high", gain);
    }
    if (msg.isCC(0x0B)) { // EQ Mid
        const gain = value / 64.0;
        deck.setParameter("eq.mid", gain);
    }
    if (msg.isCC(0x0F)) { // EQ Low
        const gain = value / 64.0;
        deck.setParameter("eq.low", gain);
    }
    
    // Filter
    if (msg.isCC(0x10)) { // Filter knob
        const cutoff = value / 127.0;
        deck.setParameter("filter.cutoff", cutoff);
    }
    
    // Volume Fader
    if (msg.isCC(0x13)) { // Channel volume
        const volume = value / 127.0;
        deck.setVolume(volume);
    }
    
    // Crossfader
    if (msg.isCC(0x1F)) { // Crossfader
        // Handle crossfader mixing between decks
        const position = (value - 64) / 64.0; // -1 to +1
        handleCrossfader(position);
    }
}

function handleCrossfader(position) {
    // Simple crossfader implementation
    const leftGain = Math.max(0, -position + 1) / 2;
    const rightGain = Math.max(0, position + 1) / 2;
    
    getDeck(1).setVolume(leftGain);
    getDeck(2).setVolume(rightGain);
}
