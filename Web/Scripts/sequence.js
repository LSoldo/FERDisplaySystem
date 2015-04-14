var msgPtr = 0;

$(function () {
    var connHub = $.connection.connectionHub;
    var stopAction = null;
    $.connection.hub.start().done(function () {
        //connHub.server.update();
    });
    connHub.client.updatesequence = function(contentList, intervalList, currentFunctionList, jsPathsList, cssPathsList) {
        content = contentList;//["aaaaa"];
        intervals = intervalList;//[100000];
        currentFunctions = currentFunctionList;//[["clock"]];
        msgPtr = 0;
        jsPaths = jsPathsList;//[[]];
        cssPaths = cssPathsList;//[["http://localhost:11527/Content/clock.css"]];

        change();
    };
    msgPtr = 0;
    
    function change() {

        if (stopAction != null) {
            clearInterval(stopAction);
        }
        var newMsg = content[msgPtr];
        document.getElementById("change").innerHTML = newMsg;
        var indexForScene, sceneLength, indexForCssInScene, cssLengthForScene, indexForJsInScene, jsLengthForScene;
        for (indexForScene = 0, sceneLength = cssPaths.length; indexForScene < sceneLength; ++indexForScene) {
            for (indexForCssInScene = 0, cssLengthForScene = cssPaths[indexForScene].length; indexForCssInScene < cssLengthForScene; ++indexForCssInScene) {
                if (indexForScene === msgPtr) {
                    loadjscssfile(cssPaths[indexForScene][indexForCssInScene], "css");
                } else {
                    removejscssfile(cssPaths[indexForScene][indexForCssInScene], "css");
                }
            }
        }
        for (indexForScene = 0, sceneLength = jsPaths.length; indexForScene < sceneLength; ++indexForScene) {
            for (indexForJsInScene = 0, jsLengthForScene = jsPaths[indexForScene].length; indexForJsInScene < jsLengthForScene; ++indexForJsInScene) {

                if (indexForScene === msgPtr) {
                    loadjscssfile(jsPaths[indexForScene][indexForJsInScene], "js");
                } else {
                    removejscssfile(jsPaths[indexForScene][indexForJsInScene], "js");
                }

            }
        }
        currentFunctions[msgPtr].forEach(function (functionName) {
            console.log(functionName);
            window[functionName]();
        });
        stopAction = setInterval(change, intervals[msgPtr]);
        msgPtr++;
        msgPtr = (msgPtr % content.length);
    }

    change();
        
    });
				