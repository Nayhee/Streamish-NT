import React, { useEffect, useState } from "react";
import Video from './Video';
import { getAllVideosWithComments, GetSearchVideos } from "../modules/videoManager";

const VideoList = () => {
  const [videos, setVideos] = useState([]);

  const getVideos = () => {
    getAllVideosWithComments().then(videos => setVideos(videos));
  };

  const search = (evt) => {
      let val = evt.target.value;
      GetSearchVideos(val)
        .then(searchVideos => {
          setVideos(searchVideos)
        });
  }

  useEffect(() => {
    getVideos();
  }, []);

  return (
    <div className="container">
      
      <div className="search-container">
        <input 
            type="text"
            placeholder="Search.."
            className="search-bar">
        </input>
        <button type="submit" onClick={search}>Submit</button>
    
      </div>
      
      <div className="row justify-content-center">
        {videos.map((video) => (
          <Video video={video} key={video.id} />
        ))}
      </div>
    </div>
  );
};

export default VideoList;