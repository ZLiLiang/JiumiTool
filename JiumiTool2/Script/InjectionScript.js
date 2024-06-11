setTimeout(() => {
    let receiver_url = "http://127.0.0.1:9743";

    function send_response_if_is_video(response) {
        if (response == undefined) return;
        if (!response["err_msg"].includes("H5ExtTransfer:ok")) return;
        let value = JSON.parse(response["jsapi_resp"]["resp_json"]);
        if (value["object"] == undefined || value["object"]["object_desc"] == undefined || value["object"]["object_desc"]["media"].length == 0) {
            return;
        }
        let media = value["object"]["object_desc"]["media"][0];
        let description = value["object"]["object_desc"]["description"].trim();
        let video_data = {
            "key": media["url"],
            "decodekey": media["decode_key"],
            "url": media["url"] + media["url_token"],
            "videolen": media["video_play_len"],
            "thumburl": media["thumb_url"],
            "size": media["file_size"],
            "description": description,
            "uploader": value["object"]["nickname"]
        };
        fetch(receiver_url, {
            method: "POST",
            mode: "no-cors",
            body: JSON.stringify(video_data),
        }).then((resp) => {
        });
    }

    function wrapper(name, origin) {
        return function () {
            let cmdName = arguments[0];
            if (arguments.length == 3) {
                let original_callback = arguments[2];
                arguments[2] = async function () {
                    if (arguments.length == 1) {
                        send_response_if_is_video(arguments[0]);
                    }
                    return await original_callback.apply(this, arguments);
                }
            } else {
            }
            let result = origin.apply(this, arguments);
            return result;
        }
    }

    window.WeixinJSBridge.invoke = wrapper("WeixinJSBridge.invoke", window.WeixinJSBridge.invoke);
    window.wvds = true;
}, 200);