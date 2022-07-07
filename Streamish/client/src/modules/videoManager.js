const baseUrl = '/api/video';
const videosWithCommentsUrl = '/api/Video/GetWithComments'
const searchVideosUrl = '/api/Video/search?q='

export const getAllVideosWithComments = () => {
    return fetch(videosWithCommentsUrl)
      .then((res) => res.json())
  };

export const addVideo = (video) => {
  return fetch(baseUrl, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(video),
  });
};

export const GetSearchVideos = (query) => {
  return fetch(searchVideosUrl + query + 'sortDesc=true')
      .then((res) => res.json())
}