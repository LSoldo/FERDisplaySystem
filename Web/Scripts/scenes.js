function html5videoscene() {
    jQuery("video, object").maximage("maxcover");
}

function imageslideshow() {
    jQuery("#maximage").maximage(
    {
        cycleOptions: {
            startingSlide: localStorage.getItem("CurrentSlide") == null ? 0 : localStorage.getItem("CurrentSlide"),
            before: function(curr, next, opts) {
                if ($("#maximage").data("cycle.opts") != undefined) {
                    localStorage.setItem("CurrentSlide", $("#maximage").data("cycle.opts").currSlide);
                }
            }
        }
    });
}

function rssscene() {
    var divs = $("div[id^=\"content-\"]").hide();
    var lastSlide = localStorage.getItem("RssSlide") == null ? 0 : localStorage.getItem("RssSlide");
    var i = lastSlide > divs.length - 1 ? 0 : lastSlide;
    (function cycle() {
        divs.eq(i).fadeIn(400)
            .delay(2000)
            .fadeOut(400, cycle);
        localStorage.setItem("RssSlide", i);
        i = ++i % divs.length;
    })();
}

function clock() {
    var monthNames = ["Siječanj", "Veljača", "Ožujak", "Travanj", "Svibanj", "Lipanj", "Srpanj", "Kolovoz", "Rujan", "Listopad", "Studeni", "Prosinac"];
    var dayNames = ["Nedjelja", "Ponedjeljak", "Utorak", "Srijeda", "Četvrtak", "Petak", "Subota"];
    var newDate = new Date();
    newDate.setDate(newDate.getDate());
    $("#Date").html(dayNames[newDate.getDay()] + " " + newDate.getDate() + ". " + monthNames[newDate.getMonth()] + " " + newDate.getFullYear() + ".");
    setInterval(function() {
        var seconds = new Date().getSeconds();
        $("#sec").html((seconds < 10 ? "0" : "") + seconds);
    }, 1000);
    setInterval(function() {
        var minutes = new Date().getMinutes();
        $("#min").html((minutes < 10 ? "0" : "") + minutes);
    }, 1000);
    setInterval(function() {
        var hours = new Date().getHours();
        $("#hours").html((hours < 10 ? "0" : "") + hours);
    }, 1000);
}