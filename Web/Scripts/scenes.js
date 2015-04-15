function html5videoscene(sequenceId) {
    jQuery("video, object").maximage("maxcover");
}

function imageslideshow(sequenceId) {
    var key = "CurrentSlide-" + sequenceId;
    jQuery("#maximage").maximage(
    {
        cycleOptions: {
            startingSlide: localStorage.getItem(key) == null ? 0 : localStorage.getItem(key),
            before: function(curr, next, opts) {
                if ($("#maximage").data("cycle.opts") != undefined) {
                    localStorage.setItem(key, $("#maximage").data("cycle.opts").currSlide);
                }
            }
        }
    });
}

function rssscene(sequenceId) {
    var key = "RssSlide-" + sequenceId;
    var divs = $("div[id^=\"content-\"]").hide();
    var lastSlide = localStorage.getItem(key) == null ? 0 : localStorage.getItem(key);
    var i = lastSlide > divs.length - 1 ? 0 : lastSlide;
    (function cycle() {
        divs.eq(i).fadeIn(400)
            .delay(2000)
            .fadeOut(400, cycle);
        localStorage.setItem(key, i);
        i = ++i % divs.length;
    })();
}

function clock(sequenceId) {
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