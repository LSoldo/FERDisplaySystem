var msgPtr = 0;
$(function () {
    var connHub = $.connection.connectionHub;
    var stopAction = null;
    $.connection.hub.start().done(function () {
        connHub.server.joinRoom(groupId);
    });
    connHub.client.updatesequence = function(setup, sequenceIdentity) {
        content = setup.HtmlContentForEveryScene;
        intervals = setup.Intervals;
        currentFunctions = setup.JsFunctionsToCall;
        jsPaths = setup.JsPathList;
        cssPaths = setup.CssPathList;
        sequenceId = sequenceIdentity;
        
        change();
    };
    if (localStorage.getItem(sequenceId) == null) {
        localStorage.setItem(sequenceId, 0);
    }

    msgPtr = sequenceId == "" || sequenceId == null ? 0 : localStorage.getItem(sequenceId);
    console.log(msgPtr);

    function change() {
        if (stopAction != null) {
            clearInterval(stopAction);           
        }
        var newMsg = content[msgPtr];
        document.getElementById("change").innerHTML = newMsg;
        var indexForScene, sceneLength, indexForCssInScene, cssLengthForScene, indexForJsInScene, jsLengthForScene;
        for (indexForScene = 0, sceneLength = cssPaths.length; indexForScene < sceneLength; ++indexForScene) {
            for (indexForCssInScene = 0, cssLengthForScene = cssPaths[indexForScene].length; indexForCssInScene < cssLengthForScene; ++indexForCssInScene) {
                if (indexForScene == msgPtr) {
                    loadjscssfile(cssPaths[indexForScene][indexForCssInScene], "css");
                } else {
                    removejscssfile(cssPaths[indexForScene][indexForCssInScene], "css");
                }
            }
        }
        for (indexForScene = 0, sceneLength = jsPaths.length; indexForScene < sceneLength; ++indexForScene) {
            for (indexForJsInScene = 0, jsLengthForScene = jsPaths[indexForScene].length; indexForJsInScene < jsLengthForScene; ++indexForJsInScene) {

                if (indexForScene == msgPtr) {
                    loadjscssfile(jsPaths[indexForScene][indexForJsInScene], "js");
                } else {
                    removejscssfile(jsPaths[indexForScene][indexForJsInScene], "js");
                }

            }
        }
        currentFunctions[msgPtr].forEach(function (functionName) {
            console.log(functionName);
            window[functionName](sequenceId);
        });
        stopAction = setInterval(change, intervals[msgPtr]);
        localStorage.setItem(sequenceId, msgPtr);
        msgPtr++;
        msgPtr = (msgPtr % content.length);
    }

    change();
        
    });
				