# MIXERX - DJ Controller Compatibility

## 🎛️ Supported DJ Controllers

MIXERX supports **ALL MIDI-capable DJ controllers** through flexible JavaScript mapping system.

### ✅ Pre-configured Controllers

| Brand | Model | Mapping File | Features |
|-------|-------|--------------|----------|
| **Pioneer DJ** | DDJ-SB3 | `pioneer-ddj-sb3.js` | 2-Deck, EQ, Filter, Sync, LED |
| **Hercules** | DJControl Inpulse 200 | `hercules-inpulse-200.js` | 2-Deck, EQ, Basic Controls |
| **Generic** | Any MIDI Controller | `generic-midi-controller.js` | Template for customization |

### 🔧 Easy Setup Controllers

These controllers work with minimal configuration:

#### **Pioneer DJ**
- DDJ-FLX4, DDJ-400, DDJ-SX series
- Use `pioneer-ddj-sb3.js` as base, adjust CC numbers

#### **Native Instruments**  
- Traktor Kontrol S2/S4, Maschine MK3
- Standard MIDI mode, use generic template

#### **Hercules**
- DJControl Inpulse 300/500, Starlight
- Similar to Inpulse 200 mapping

#### **Numark**
- Party Mix, DJ2GO2, Mixtrack Pro series
- Use generic template with Numark CC mapping

#### **Denon DJ**
- MC4000, MC6000MK2, Prime GO
- MIDI mode required, use generic template

#### **Reloop**
- Beatmix 2/4, Terminal Mix series
- Standard MIDI compatibility

#### **Behringer**
- CMD Studio 4a, BCD3000
- Use generic template

### 🛠️ Custom Controller Setup

**3-Step Process:**

1. **Connect Controller** via USB
2. **Load Generic Template** in MIXERX
3. **Customize Mapping** using MIDI monitor

```javascript
// Example: Map your play button
if (msg.isNoteOn(0x20)) { // Replace 0x20 with your button's note
    deck.playPause();
}
```

### 📱 Mobile/Tablet Controllers

| App | Platform | Compatibility |
|-----|----------|---------------|
| **TouchOSC** | iOS/Android | ✅ Full MIDI support |
| **Lemur** | iOS/Android | ✅ Advanced scripting |
| **MIDI Designer** | iOS | ✅ Custom layouts |
| **Control Surface** | Android | ✅ Basic controls |

### 🎹 Keyboard Controllers

Any MIDI keyboard can be used as DJ controller:
- **Akai MPK series** - Pads + Knobs
- **Novation Launchkey** - Pads + Faders  
- **Arturia KeyLab** - Premium controls
- **M-Audio Oxygen** - Budget option

### 🔌 Hardware Requirements

**Minimum:**
- USB MIDI interface
- Standard MIDI protocol support
- Windows 10+ or macOS 10.15+

**Recommended:**
- USB bus-powered controller
- LED feedback support
- Touch-sensitive knobs/faders
- Dedicated transport buttons

### 🚀 Advanced Features

**Supported via JavaScript:**
- **LED Feedback** - Button/pad illumination
- **Display Updates** - LCD/OLED screen control
- **Multi-deck Mapping** - Up to 4 decks
- **Custom Functions** - Loops, samples, effects
- **Crossfader Curves** - Linear/logarithmic
- **Shift Functions** - Secondary button functions

### 📝 Creating Custom Mappings

1. **Identify MIDI Messages:**
   ```bash
   # Use MIDI monitoring tools
   - Windows: MIDI-OX, MIDIBERRY
   - macOS: MIDI Monitor, SnoizeMIDI
   - Cross-platform: QjackCtl
   ```

2. **Map Controls:**
   ```javascript
   function onMidiMessage(msg) {
       if (msg.isCC(0x10)) { // Your knob CC number
           getDeck(1).setParameter("eq.high", msg.value / 127.0);
       }
   }
   ```

3. **Test & Refine:**
   - Load mapping in MIXERX
   - Test each control
   - Adjust sensitivity/ranges
   - Add LED feedback

### 🆘 Troubleshooting

**Controller Not Detected:**
- Check USB connection
- Install manufacturer drivers
- Enable MIDI mode on controller
- Restart MIXERX

**Controls Not Working:**
- Verify MIDI messages with monitor
- Check CC/Note numbers in mapping
- Ensure correct MIDI channel
- Test with generic template

**No LED Feedback:**
- Controller must support MIDI input
- Check `sendMidi()` calls in mapping
- Verify LED CC numbers
- Some controllers need special initialization

### 📞 Support

**Need help with your controller?**
1. Check existing mappings in `/mappings/` folder
2. Use generic template as starting point
3. Join MIXERX community for mapping help
4. Submit your working mapping to help others!

---

**🎵 Any MIDI controller can become a professional DJ controller with MIXERX!**
