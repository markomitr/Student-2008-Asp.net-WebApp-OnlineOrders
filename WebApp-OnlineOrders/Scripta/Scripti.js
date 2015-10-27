function proveriVlez(x) {
    x.focus();
    if (x.value.trim() == "0") {
        x.value = "";
    }
    else {
        x.select();
    }

}
function proveriIzlez(x) {
    if (x.value.trim() == "") {
        x.value = "0";
    }
    else {
        if (isNaN(x.value)) {
            x.value = "0";
            x.focus();
            alert("Внесете целобројна вредност");
            x.value = "0";
            x.focus();
        }
        else {

            x.value = parseInt(x.value, 10);
            if (x.value < 0) {
                x.value = "0";
                x.focus();
            }
        }
    }
}
$(document).ready(function() {
 $('#slikiReklami').cycle('fade');
try{
    var url = location.search;
    url = url.replace('?', '');
    var tabeli = url.split('&');
    var prv = tabeli[0].split('=');
    var vtor = tabeli[1].split('=');

    if (prv[1] == 2 && vtor[1] == 2) {
        var win_w = $(document).width();
        var win_h = $(document).height();
        $('#slikiReklami').addClass('hide');
        $('#shadow').css('background-color', '#000');
        $('#shadow').css({
            position: 'absolute',
            top:'0px',
            left:'0px',
            width: '100%',
            height: $(document).height() + $('#narackaLista').height(),
            opacity: '.95'
        });
        
        $('#narackaLista').css({
            position:'absolute',
            top:win_h/2-300+'px',
            left:win_w/2-400+'px',
            width:'800px',
            border: '1px solid black',
        });
         $('#narackaLista').css('z-index','10');
         $('#narackaLista').css('background-color','white');
         $('#narackaLista').slideDown('slow');
         $("tr:even").css("background-color", "#D0D0D0");
         $("tr:odd").css("background-color", "#FFFFFF");
    $('#shadow').click(function(){

    window.location ='?Mod=2&A=1';

    });
    
    }
    
    $('#podatociDiv tr').hover(function() {
        $('#podatociDiv .selektiranRed').removeClass('selektiranRed');
        $(this).addClass('selektiranRed');
    });
    }catch(Error)
    {
    }
});
var proba = 0;
function pageLoad() {
    var pom = document.getElementById("slikiReklami");
    if (pom == null) {
        proba = 0
    }else if (pom != null && proba != 2) {
    proba = 1;
    }
    if (Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack() && proba == 1) {
        $('#slikiReklami').cycle('fade');
        proba = 2;
    }
}
function prikazi(x){
    $('#slikiReklami').cycle('pause');
    $('#' + x).removeClass('hide');
    $('#' + x).addClass('show');
}
function iscisti(x) {
    $('#slikiReklami').cycle('resume');
    $('#' + x).removeClass('show');
    $('#' + x).addClass('hide');
}