// SignalR Methods
$(function () {
    var chatHub = $.connection.chatHub; // SignalR Class Name = ChatHub
    $.connection.hub.start().done(function () // Connection method
    {
        chatHub.server.connect();
    });

    chatHub.client.onConnected = function (username) // On connected method
    {
        console.log(username + " connected!");
    };

    chatHub.client.sendPrivateMessage = function (toUserName, msg, time, avatar) // After sending message to do
    {
        var result = "";
        result += "<div class='main-message-box st3'> <div class='message-dt st3'><div class='message-inner-dt'><p>" + msg
            + "</p></div><span>" + time + "</span></div><div class='messg-usr-img'><img src='/Content/images/WebUsersPP/"
            + avatar + "' alt='" + toUserName + "'></div></div>";
        $('#mCSB_1_container').append(result);
        $('.messages-line').mCustomScrollbar('scrollTo', 'bottom');
    };

    chatHub.client.addMessage = function (myUserName, msg, time, avatar) // add message
    {
        var result = "";
        result += "<div class='main-message-box ta-right'> <div class='message-dt'><div class='message-inner-dt'><p>" + msg
            + "</p></div><span>" + time + "</span></div><div class='messg-usr-img'><img src='/Content/images/WebUsersPP/"
            + avatar + "' alt='" + myUserName + "'></div></div>";
        $('#mCSB_1_container').append(result);
        $('.messages-line').mCustomScrollbar('scrollTo', 'bottom');
    };

    var typingListener;
    chatHub.client.setTyping = function (fromUserName) // seting typing text
    {
        var name = $("#sender-username").text();
        if (name == fromUserName)
        {
            $("#sender-status").text('Typing...');
            clearTimeout(typingListener);
            typingListener = setTimeout(function () {
                $("#sender-status").text('Online');
            }, 5000);
        }
    };

    $("#sendMessageButton").click(function () // Send button click
    {
        var toUserName = $('#sender-username').text();
        var msg = $('#messageInput').val();
        if (msg == "") return;
        chatHub.server.sendPrivateMessage(toUserName, msg);
        $('#messageInput').val("");
    });

    $('#messageInput').keypress(function (e) // keypress on message input
    { 
        if (e.which == 13) //enter
        {
            $("#sendMessageButton").click();
            return false;
        }
        else
        {
            var toUserName = $("#sender-username").text();
            chatHub.server.setTyping(toUserName);
        }
    });



    chatHub.client.onUserDisconnected = function (connectionId, userName) // On User Disconnected
    {
        var disUserName = $('.usr-mg-info #sender-username').text();
        if (disUserName == userName) {
            $('#sender-status').text("Offline");
        }
        $('#posted_time-' + userName).text("Offline");
        console.log(userName + " disconnected!");
    };

    chatHub.client.onUserConnected = function (connectionId, userName) // On User Connected
    {
        var conUserName = $('.usr-mg-info #sender-username').text();
        if (conUserName == userName) {
            $('#sender-status').text("Online");
        }
        $('#posted_time-' + userName).text("Online");
        console.log(userName + " connected!");
    };


});

