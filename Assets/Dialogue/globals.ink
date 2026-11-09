VAR flirt_yuri = 0
VAR Scandal = 0
VAR scenechange = 0

//MUSIC
VAR menu_theme = 0 
VAR intro_gardenambience = 0 //to play in the intro scene in the garden
VAR courtroom_theme = 0 //generic courtroom music

//SFX

//UI (I don't know that you'll actually need these as dialogue team but I'm adding them to be safe or in case)
VAR button_hover = 0
VAR button_press = 0
VAR pause_close = 0
VAR pause_open = 0
VAR textscroll = 0
VAR textscroll_alt = 0

//General SFX

VAR cough = 0
VAR crowdchatter = 0
VAR crowdgasp = 0
VAR crowdrising = 0
VAR crowdwalking = 0
VAR paper1 = 0
VAR paper2 = 0
VAR gavelbang = 0
VAR footsteps_fast = 0
VAR footsteps_boots = 0
VAR footsteps_heels = 0

//Chapter 1 SFX

VAR looncall = 0
VAR tea_pour = 0




//FUNCTIONS SECTION FOR SOME REASON
=== function abs(x) ===
{ x < 0:
    ~ return -x
- else:
    ~ return x
}
=== function round(x) ===
    ~ return FLOOR(x + 0.5)

/Variables needed: Background change, sfx, music, slice sound, french cheering, other stuff for a guillitine ending