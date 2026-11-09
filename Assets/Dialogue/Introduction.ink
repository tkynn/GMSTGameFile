INCLUDE globals.ink
->Start
===Start===
#speaker:Narrator # portrait:portrait0 # layout:right
It is April 25th, 1776. Anna Larpent takes a stroll through the English Garden in London, during a beautiful sunrise at 7 O’clock, looking out for a special someone that called her here...
+[Begin]->Begin
====Begin===
#speaker:Mysterious Lady # portrait:portrait2 # layout:Right
“Anna, over here!"
#speaker:Narrator # portrait:portrait0 # layout:right
Anna hears a voice calling out to her, so sweet like a honey suckle. Her head whips over to gaze at who called her and found a woman dressed in off-white sitting around a tea set.
#speaker:Anna Larpent # portrait:portrait1 # layout:Left
“Oh goodness! Gertrude! My dear friend, it’s been so long!"
#speaker:Gertrude # portrait:portrait2 # layout:Right
“I know! My travels across the English Isles have left me wanting my dear friend’s presence! I’ve heard from grapevine that there was a grand trial going on and you, darling, were there present” 
“Please recount your tales to me, for I am most curious!”
#speaker:Anna Larpent # portrait:portrait1 # layout:Left
“But of course, dear Gertrude!”
#speaker:Narrator # portrait:portrait0 # layout:right
Anna takes a seat across from Gertrude and debates taking a sip of the tea available.
+[Tea]
~flirt_yuri = 3
->Tea
+[No Tea, just <i><b>Tea</b></i>]->edging
====Tea===
#speaker:Anna Larpent # portrait:portrait1 # layout:Left
 “Still knowing my tea preference, I see New Jersey Tea, with two cubes of sugar.” 
#speaker:Narrator # portrait:portrait0 # layout:Left
 Gertrude nods knowingly, she wouldn’t forget her best friend’s tea combination. She takes a sip of the herbal tea, the dark roasted leaves mimicking the taste of black tea from Qing’s Dynasty. She clears her throat and then slams her hands on the table and stands up. 
 #speaker:Gertrude # portrait:portrait2 # layout:Right
 “Now enough teasing!! You know of which reason I return to the great city of London! The Great Bigamy Trial of Elizabeth Chudleigh! Such tension, such drama! Please, dear! You cannot keep me waiting for much longer!!”
+[Story time!]->edging
====edging===
#speaker:Narrator # portrait:portrait0 # layout:right
Anna smiles and holds her hand up.
#speaker:Anna Larpent # portrait:portrait1 # layout:Left
“Alright, my dear Gertrude. I shan't keep you waiting. Take your seat and listen closely..."
+[Continue]
~scenechange = 1
->Procession
====Procession===
#speaker:Anna Larpent # portrait:portrait1 # layout:Left
“On the day the Duchess of Kingston was tried for Bigamy, April 15th, at about eleven entered the procession. Twas an elegant sight indeed! First came the Lord High Steward with his many men all well dressed!!” 
	“Then came the Clerks of the council..”
	“Soon followed by the Masters in chancery in their gowns and bands!”
	“Then came the Judges in their lavish scarlett robes and caps!”
	“The Bishops came next, which I must say that their robes were particularly ungraceful and not appealing at all!” 
“Then came the Dukes in their most elegant robes.”
“Finally came Elizabeth Chudleigh with her three ladies, all in black gowns. She seemed unconcerned about this trail, but I must say, she really looked handsome!” 
#speaker:Gertrude # portrait:portrait2 # layout:Right
 “Oh good heavens! How riveting, darling!!”
#speaker:Anna Larpent # portrait:portrait1 # layout:Left
“At last, my dear friend, this is how the first day of the trial finally begins!”
~flirt_yuri = flirt_yuri+2.5
->DONE