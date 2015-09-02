using System.Collections.Generic;

public interface IStorageAdapter
{
    List<Post> GetAllPosts();
    void Save(Post post);
    void Delete(Post post);
}