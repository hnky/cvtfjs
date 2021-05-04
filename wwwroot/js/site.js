// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const threshold = 0.75;
const label_file = '/models/labels.txt'
const model_file = '/models/model.json'

async function load_labels() {
    return fetch(label_file)
        .then(function (response) {
            return response.text();
        }).then(function (data) {
            var lines = data.split(/\n/);
            return lines;
        })
}

async function load_model() {
    const model = new cvstfjs.ClassificationModel();
    await model.loadModelAsync(model_file);
    return model;
}

function sleep(time) {
    return new Promise((resolve) => setTimeout(resolve, time))
}

predict = (video, model, labels) => {

    model.executeAsync(video).then(predictions => {
        this.renderPredictions(predictions, labels);
        sleep(1000).then(() => {
            this.predict(video, model, labels);
        })
    });

};

renderPredictions = (predictions, labels) => {
    let pred = predictions[0]
    if (Math.max(...pred) > threshold) {
        document.getElementById("label").innerHTML = labels[pred.indexOf(Math.max(...pred))];
    } else {
        document.getElementById("label").innerHTML = "unkown"
    }
};

if (document.querySelector("#frame") && navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
    this.videoRef = document.querySelector("#frame");

    const webCamPromise = navigator.mediaDevices
        .getUserMedia({
            audio: false,
            video: {
                facingMode: "user"
            }
        })
        .then(stream => {
            window.stream = stream;
            this.videoRef.srcObject = stream;
            return new Promise((resolve, reject) => {
                this.videoRef.onloadedmetadata = () => {
                    resolve();
                };
            });
        });

    const modelPromise = load_model();
    const labelPromise = load_labels();

    Promise.all([webCamPromise, modelPromise, labelPromise])
        .then(values => {
           this.predict(this.videoRef, values[1], values[2]);
        })
        .catch(error => {
            console.error(error);
        });
}