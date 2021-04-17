var answers = {};
function compile(req, type, host, sitekey, n, value1, value2)
{
   return JSON.stringify({
        v: "",
        answers: answers,
        c: '{"type":"' + type + '","req":"' + req + '"}',
        job_mode: "image_label_binary",
        motionData: '{"st":' + value1 + ',"dct":' + value2 + ',"mm":[]}',
        n: n,
        serverdomain: host,
        sitekey: sitekey
   });
}