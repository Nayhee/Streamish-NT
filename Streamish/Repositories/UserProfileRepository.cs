using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }


        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT * FROM UserProfile";

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var users = new List<UserProfile>();
                        while(reader.Read())
                        {
                            users.Add(new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                            });
                        }
                        return users;
                    }
                }
            }
        }

        public UserProfile GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT * FROM UserProfile WHERE Id=@id";
                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile user = null;
                        if(reader.Read())
                        {
                            user = new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                            };
                        }
                        return user;
                    }
                }
            }
        }

        public UserProfile GetUserByIdWithVideos(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT u.Id as UserId, u.Name, u.Email, u.ImageUrl, u.DateCreated,
                                        v.Id as VidId, v.Title, v.Description, v.Url, v.DateCreated as VideoDateCreated, v.UserProfileId,
                                        c.Id as CommentId, c.Message, c.VideoId, c.UserProfileId as UId
                                        FROM UserProfile u
                                        Left JOIN Video v on v.UserProfileId=u.Id
                                        Left JOIN Comment c on c.VideoId=v.Id
                                        WHERE u.Id = @id
                    ";
                    DbUtils.AddParameter(cmd, "@Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile user = null;

                        while(reader.Read())
                        {
                            var userId = DbUtils.GetInt(reader, "UserId");

                            if(user == null)
                            {
                                user = new UserProfile()
                                {
                                    Id = id,
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                    DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                    Videos = new List<Video>()
                                };
                            }

                            if(DbUtils.IsNotDbNull(reader, "VidId"))
                            {
                                int currentVideoId = DbUtils.GetInt(reader, "VidId");
                                var existingVideo = user.Videos.FirstOrDefault(p => p.Id == currentVideoId);

                                if(existingVideo == null)
                                {
                                    existingVideo = new Video()
                                    {
                                        Id = currentVideoId,
                                        Title = DbUtils.GetString(reader, "Title"),
                                        Description = DbUtils.GetString(reader, "Description"),
                                        DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                        Url = DbUtils.GetString(reader, "Url"),
                                        UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                        Comments = new List<Comment>()
                                    };
                                    user.Videos.Add(existingVideo);
                                }
                                
                                if (DbUtils.IsNotDbNull(reader, "CommentId"))
                                {
                                    Comment comment = new Comment()
                                    {
                                        Id = DbUtils.GetInt(reader, "CommentId"),
                                        Message = DbUtils.GetString(reader, "Message"),
                                        VideoId = DbUtils.GetInt(reader, "VideoId"),
                                        UserProfileId = DbUtils.GetInt(reader, "UId")
                                    };

                                    user.Videos.FirstOrDefault(p => p.Id == existingVideo.Id).Comments.Add(comment);
                                }
                            }
                        }
                        return user;
                    }
                }
            }
        }

        public void Add(UserProfile userProfile)
        {
            using(var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" INSERT INTO UserProfile (Name, Email, ImageUrl, DateCreated)
                                         OUTPUT INSERTED.ID
                                         VALUES (@Name, @Email, @ImageUrl, @DateCreated) 
                    ";
                    DbUtils.AddParameter(cmd, "@Name", userProfile.Name);
                    DbUtils.AddParameter(cmd, "@Email", userProfile.Email);
                    DbUtils.AddParameter(cmd, "@ImageUrl", userProfile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@DateCreated", userProfile.DateCreated);

                    userProfile.Id = (int)cmd.ExecuteScalar();
                }
            }
        }
        public void Update(UserProfile userProfile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        UPDATE UserProfile
                                        SET Name = @Name,
                                            Email = @Email,
                                            ImageUrl = @ImageUrl,
                                            DateCreated = @DateCreated
                                        WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Name", userProfile.Name);
                    DbUtils.AddParameter(cmd, "@Email", userProfile.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", userProfile.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", userProfile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Id", userProfile.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(int id)
        {
            using(var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@Id", id);
                }
            }
        }
    }
}
