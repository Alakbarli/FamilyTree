$("#addPhoto").click(function () {
    $("#PhotoFile").click();
})
$("#addPhoto").click(function () {
    $("#LogoFile").click();
})
$("#PhotoFile").change(function (event) {
    if ($(this).val() != "") {
        if ([...event.target.files][0].type.match("image/*")) {
            let reader = new FileReader;
            reader.onloadend = function (rd) {
                $("#profilePhoto img").attr("src", rd.target.result);
            }
            reader.readAsDataURL([...event.target.files][0]);
        }
        $("#profilePhoto img").removeClass("d-none");
        $("#addPhoto").text("Edit photo");
    }
    else {
        $("#profilePhoto img").attr("src", "");
        $("#profilePhoto img").addClass("d-none");
        $("#addPhoto").text("Add photo");
    }

});

$("#LogoFile").change(function (event) {
    if ($(this).val() != "") {
        if ([...event.target.files][0].type.match("image/*")) {
            let reader = new FileReader;
            reader.onloadend = function (rd) {
                $("#profilePhoto img").attr("src", rd.target.result);
            }
            reader.readAsDataURL([...event.target.files][0]);
        }
        $("#profilePhoto img").removeClass("d-none");
        $("#addPhoto").text("Edit photo");
    }
    else {
        $("#profilePhoto img").attr("src", "");
        $("#profilePhoto img").addClass("d-none");
        $("#addPhoto").text("Add photo");
    }

});

RunPopUp("usePopup", "href", true);

$(document).ready(function () {
    let monitor = $("#monitor");
    let board = $("#board");
    board.css("left", -(board.innerWidth() - monitor.innerWidth()) / 2 + "px");
    //  $(document).on("click","#gotop",function(){
    //     let top=board.css("top");
    //     top=parseInt(top);
    //     top=top+70;
    //     board.css("top",top+"px");
    // })
    // $(document).on("click","#goright",function(){
    //     let left=board.css("left");
    //     left=parseInt(left);
    //     left=left-70;
    //     board.css("left",left+"px");
    // })
    // $(document).on("click","#gobottom",function(){
    //     let top=board.css("top");
    //     top=parseInt(top);
    //     top=top-70;
    //     board.css("top",top+"px");
    // })
    // $(document).on("click","#goleft",function(){
    //     let left=board.css("left");
    //     left=parseInt(left);
    //     left=left+70;
    //     board.css("left",left+"px");
    // }) 
    $(document).on("click", "#zoomin", function () {
        let zoom = board.css("transform");
        zoom = zoom.substring(7, zoom.indexOf(","));
        zoom = +zoom;
        if (zoom < 1.1) {
            zoom = zoom + 0.1;
            board.css('transform', `scale(${zoom})`);
        }
    })
    $(document).on("click", "#zoomout", function () {
        let zoom = board.css("transform");
        zoom = zoom.substring(7, zoom.indexOf(","));
        zoom = +zoom;
        if (zoom > 0.8) {
            zoom = zoom - 0.1;
            board.css('transform', `scale(${zoom})`);
        }
    })
    board.draggable();
    let top = parseInt(board.css("top"));
    let left = parseInt(board.css("left"));
    //  let ofx=0;
    // let ofy=0;
    //let width = board.innerWidth() - monitor.innerWidth();
    //let height = board.innerHeight() - monitor.innerHeight();
    board.draggable();
    monitor.mousedown(function (e) {
        //  ofx=e.offsetX
        // ofy=e.offsetY;

        grabbing = true
        monitor.css("cursor", "grabbing");

    });
    monitor.mouseup(function () {
        let width = board.innerWidth() - monitor.innerWidth();
        let height = board.innerHeight() - monitor.innerHeight();
        top = board.css("top");
        left = board.css("left");
        top = parseInt(top);
        left = parseInt(left);
        if (top > 0) {
            grabbing = false;
            board.css("top", 0 + "px");
        }
        if (top < -height) {
            board.css("top", -height + "px");
            grabbing = false;
        }
        if (left > 0) {
            board.css("left", 0 + "px");
            grabbing = false;
        }
        if (left < -width) {
            board.css("left", -width + "px");
            grabbing = false;
        }
        monitor.css("cursor", "grab");
        grabbing = false;

    });

    monitor.mousemove(function (e) {
        //if(grabbing==true){
        // grabbing=false;
        // if(top+(e.offsetY-ofy)<=0&&left+(e.offsetX-ofx)<=0&&top+(e.offsetY-ofy)>-height&&left+(e.offsetX-ofx)>-width){
        //     board.css("top",top+(e.offsetY-ofy)+"px");
        //     board.css("left",left+(e.offsetX-ofx)+"px");
        // }
        // else{
        //     grabbing=false;
        // }

        // }

    });
    monitor.mouseout(function () {
        let width = board.innerWidth() - monitor.innerWidth();
        let height = board.innerHeight() - monitor.innerHeight();
        top = board.css("top");
        left = board.css("left");
        top = parseInt(top);
        left = parseInt(left);
        if (top > 0) {
            grabbing = false;
            board.css("top", 0 + "px");
        }
        if (top < -height) {
            board.css("top", -height + "px");
            grabbing = false;
        }
        if (left > 0) {
            board.css("left", 0 + "px");
            grabbing = false;
        }
        if (left < -width) {
            board.css("left", -width + "px");
            grabbing = false;
        }
        monitor.css("cursor", "grab");
        grabbing = false;
        grabbing = false;
        monitor.css("cursor", "grab");
    })

    $(".morePerson").click(function () {


        if ($(this).parent().next().hasClass("d-flex")) {
            $(this).parent().next().toggleClass("clipPath");
            $(this).children().first().toggleClass("rotate-180");
            setTimeout(() => {
                $(this).parent().next().toggleClass("d-flex");
            }, 1000);
        }
        else {
            $(this).parent().next().toggleClass("d-flex");
            setTimeout(() => {
                $(this).parent().next().toggleClass("clipPath");
                $(this).children().first().toggleClass("rotate-180");
            }, 0.001);

        }

    });


    //CSHTML
    $(".persondiv .image img").each(function () {
        let src = $(this).attr("src");
        let srcp = $("#srcPhoto").attr("src");
        $(this).attr("src", `${srcp + src}`);

    });
    $(".persondiv").each(function () {
        let id = $(this).attr("data-id");
        let familyId = $(this).attr("data-familyid");
        let urlp = $("#urlPerson").attr("href");
        $(this).attr(`href`, `${urlp + "/" + id + "?" + "familyId=" + familyId}`);

    });


});

