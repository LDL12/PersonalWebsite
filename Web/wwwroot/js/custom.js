$.extend({
    custom: {
        post: function (url, body, func)
        {
            fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(body)
            })
                .then(response => response.json())
                .then(result =>
                {
                    func(result);
                });
        }
    }
});