public var theTexture : Texture2D;
private var alpha : float = 0;
var duration: float = 1;
function Start(){
    yield FadeIn();
    yield FadeOut();
    yield NextLevel();
}
function FadeIn(){
var d = 0.3f / duration;
    while( alpha < 1 ){ alpha += Time.deltaTime * d; yield; }
}
function FadeOut(){
var d = 0.3f / duration;
    while( alpha > 0 ){ alpha -= Time.deltaTime * d; yield; }
}
function NextLevel(){
    yield FadeOut();
    Application.LoadLevel("SceneMenu");
}
function OnGUI(){
    GUI.color = Color(1,1,1, alpha );
    GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height), theTexture);
}