var w=1196,h=609;
var a=[12/w*100,194/h*100,533/w*100,245/h*100,328/w*100,400/h*100,473/w*100,449/h*100,640/w*100,516/h*100,924/w*100,566/h*100];
setCoords();
function setCoords()
{    var width=parseFloat($('#image').css('width'));
    var height=parseFloat($('#image').css('height'));
    var d=[];
   a.forEach((value,index)=>{
       if((index%2)==0){
        
        d.push((value*width)/100);
       }
       else{
        //console.log(index);
        d.push((value*height)/100);
       }
   })
   //console.log(a);
   //console.log(d);
   $('#cost').attr('coords',`${d[0]},${d[1]},${d[2]},${d[3]}`);
   $('#learn_more').attr('coords',`${d[4]},${d[5]},${d[6]},${d[7]}`);
   $('#discount').attr('coords',`${d[8]},${d[9]},${d[10]},${d[11]}`);
}
   $(window).resize(()=>{
       setCoords();
   });
(function(){
    $('.display_none').hide();
})();
function color(){
    setTimeout(function(){ 
    $('.navs-color').css('background-color','rgb(243, 237, 237)');
    $('.navs-color').css('color','dodgerblue');
        $('.navs-color').css({ 'border-bottom': '1px solid rgb(243, 237, 237)', 'margin-top': '5px' });    
        $('.navs-color.active').css('background-color', '');
        $('.navs-color.active').css('background-color','#3498db');
        $('.navs-color.active').css('color','white');
        $('.navs-color.active').css({ 'border-bottom': '2px solid red' });
      
    }, 10);
    }
    color();
function tab_toggle(obj){
    setTimeout(function(){ 
    $('.mostly_searched').attr('class','btn btn-outline-primary m-1 mostly_searched');
    $(obj).attr('class','btn btn-outline-primary m-1 mostly_searched active');
}, 10);
}
var pre=$('#navListItem').html();
var count=1;
function more(id) {
    if (count % 2 == 0) {
        
            $("#navListItem").html('<li class="nav-item mx-1 mb-0">' +
                '<a class="nav-link navs-color navachPaddig active" data-toggle="tab" href="#home"  onclick="color()">Overview</a>' +
                ' </li>' +
                '<li class="nav-item mx-1 mb-0">' +
                '<a class="nav-link navs-color navachPaddig" data-toggle="tab" href="#menu1" onclick="color()">Side Effects</a>' +
                '</li>' +
                '<li class="nav-item mx-1 mb-0">' +
                '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu2" onclick="color()">Dosage</a>' +
                '</li>' +
                '<li class="nav-item mx-1 mb-0">' +
                '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu3" onclick="color()">Professional</a>' +
                '</li>' +
                '<li class="nav-item mx-1 mb-0">' +
                '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu4" onclick="color()">Tips</a>' +
                '</li>' +
                '<li class="nav-item mx-1 mb-0">' +
                '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu5" onclick="color()">Interaction</a>' +
                '</li> ').show('slow');
            $('#more_less').html(' <i class="fa fa-plus-square colorblue"></i>');
      

       
            //document.getElementById('navListItem').innerHTML = (
            //    '<li class="nav-item mx-1 mb-0">' +
            //    '<a class="nav-link navs-color navachPaddig active" data-toggle="tab" href="#home"  onclick="color()">Overview</a>' +
            //    ' </li>' +
            //    '<li class="nav-item mx-1 mb-0">' +
            //    '<a class="nav-link navs-color navachPaddig" data-toggle="tab" href="#menu1" onclick="color()">Side Effects</a>' +
            //    '</li>' +
            //    '<li class="nav-item mx-1 mb-0">' +
            //    '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu2" onclick="color()">Dosage</a>' +
            //    '</li>' +
            //    '<li class="nav-item mx-1 mb-0">' +
            //    '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu3" onclick="color()">Professional</a>' +
            //    '</li>' +
            //    '<li class="nav-item mx-1 mb-0">' +
            //    '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu4" onclick="color()">Tips</a>' +
            //    '</li>' +
            //    '<li class="nav-item mx-1 mb-0">' +
            //    '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu5" onclick="color()">Interaction</a>' +
            //    '</li> <a href="#" id="more_less" onclick="more(navListItem)"> <i class="fa fa-plus-square colorblue"></i></a>'); 
       
       
    }
    else {
        
        document.getElementById('navListItem').innerHTML+=(
            '<li class="nav-item mx-1 mb-0">'+
        '<a class="nav-link navs-color navachPaddig" data-toggle="tab" href="#home"  onclick="color()">Overview</a>'+
     ' </li>'+
      '<li class="nav-item mx-1 mb-0">'+
        '<a class="nav-link navs-color navachPaddig" data-toggle="tab" href="#menu6" onclick="color()">Side Effects</a>'+
      '</li>'+
      '<li class="nav-item mx-1 mb-0">'+
        '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu7" onclick="color()">Dosage</a>'+
      '</li>'+
      '<li class="nav-item mx-1 mb-0">'+
        '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu8" onclick="color()">Professional</a>'+
      '</li>'+
      '<li class="nav-item mx-1 mb-0">'+
        '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu9" onclick="color()">Tips</a>'+
      '</li>'+
      '<li class="nav-item mx-1 mb-0">'+
        '<a class="nav-link  navs-color navachPaddig" data-toggle="tab" href="#menu10" onclick="color()">Interaction</a>'+
      '</li> ');
            $('#more_less').html(' <i class="fa fa-minus colorblue"></i>');
        
    }
    color();
    count++;

}