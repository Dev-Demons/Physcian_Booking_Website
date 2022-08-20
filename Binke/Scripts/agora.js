/**
 * @name handleFail
 * @param err - error thrown by any function
 * @description Helper function to handle errors
 */
let handleFail = function (err) {
    console.log("Error : ", err);
};

// Queries the container in which the remote feeds belong
let remoteContainer = document.getElementById("remote-container");
/**
 * @name addVideoStream
 * @param streamId
 * @description Helper function to add the video stream to "remote-container"
 */
function addVideoStream(streamId) {
    let streamDiv = document.createElement("div"); // Create a new div for every stream
    streamDiv.id = streamId;                       // Assigning id to div
    streamDiv.style.transform = "rotateY(180deg)"; // Takes care of lateral inversion (mirror image)
    remoteContainer.appendChild(streamDiv);      // Add new div to container
}
/**
 * @name removeVideoStream
 * @param evt - Remove event
 * @description Helper function to remove the video stream from "remote-container"
 */
function removeVideoStream(streamId) {
    let remDiv = document.getElementById(streamId);
    if (remDiv) remDiv.parentNode.removeChild(remDiv);
}

// Client Setup
// Defines a client for RTC
let client = AgoraRTC.createClient({
    mode: 'rtc',
    codec: "vp8"
});

// Client Setup
// Defines a client for Real Time Communication
client.init("0f8f282b530c4232b289867cda582737", () => console.log("AgoraRTC client initialized"), handleFail);

client.join(null, "any-channel", null, (uid) => {

    // Stream object associated with your web cam is initialized
    let localStream = AgoraRTC.createStream({
        streamID: uid,
        audio: true,
        video: true,
        screen: false
    });

    // Associates the stream to the client
    localStream.init(function () {

        //Plays the localVideo
        localStream.play('me');

        //Publishes the stream to the channel
        client.publish(localStream, handleFail);

    }, handleFail);

}, handleFail);

//When a stream is added to a channel
client.on('stream-added', function (evt) {
    client.subscribe(evt.stream, handleFail);
});
//When you subscribe to a stream
client.on('stream-subscribed', function (evt) {
    let stream = evt.stream;
    let streamId = String(stream.getId());
    addVideoStream(streamId);
    stream.play(streamId);
});

//When a person is removed from the stream
client.on('stream-removed', (evt) => {
    let stream = evt.stream;
    let streamId = String(stream.getId());
    stream.close();
    removeVideoStream(streamId);
});
client.on('peer-leave', (evt) => {
    let stream = evt.stream;
    let streamId = String(stream.getId());
    stream.close();
    removeVideoStream(streamId);
});
