window.infiniteScroll = {
    observe: function (sentinel, dotNetRef) {
        const observer = new IntersectionObserver(entries => {
            if (entries[0].isIntersecting) {
                dotNetRef.invokeMethodAsync('OnSentinelVisible');
            }
        }, { threshold: 0.1 });
        observer.observe(sentinel);
        return observer;
    }
};