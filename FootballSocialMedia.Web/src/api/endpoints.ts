import { api, unwrap } from "./client";
import type {
  AppNotification, AutoComplete, ChatMessage, Comment, Conversation, Follow, LikeResult, News, Poll, Post,
  SearchResult, TokenDto, UserDto, UserProfile, UserSummary,
} from "./types";

export type FeedMode = "all" | "following";
export const PAGE_SIZE = 10;

export const authApi = {
  login: (emailOrUserName: string, password: string) =>
    unwrap<TokenDto>(api.post("/auth/login", { email: emailOrUserName, password })),
  register: (userName: string, email: string, password: string) =>
    unwrap<UserDto>(api.post("/auth/register", { userName, email, password })),
  revoke: (refreshToken: string) => api.post("/auth/revoke-refresh-token", { token: refreshToken }),
};

export const usersApi = {
  me: () => unwrap<UserProfile>(api.get("/users/me")),
  profile: (id: string) => unwrap<UserProfile>(api.get(`/users/${id}/profile`)),
  search: (q: string) => unwrap<UserSummary[]>(api.get("/users/search", { params: { q } })),
  suggestions: (take = 5) => unwrap<UserSummary[]>(api.get("/users/suggestions", { params: { take } })),
  updateMe: (form: { userName?: string; email?: string; favoriteTeam?: string; bio?: string; profileImage?: File | null }) => {
    const data = new FormData();
    if (form.userName) data.append("UserName", form.userName);
    if (form.email) data.append("Email", form.email);
    data.append("FavoriteTeam", form.favoriteTeam ?? "");
    data.append("Bio", form.bio ?? "");
    if (form.profileImage) data.append("ProfileImage", form.profileImage);
    return unwrap<UserProfile>(api.put("/users", data));
  },
};

export const postsApi = {
  feed: (mode: FeedMode, page: number) =>
    unwrap<Post[]>(api.get("/posts/feed", { params: { mode, page, pageSize: PAGE_SIZE } })),
  byUser: (userId: string, page: number) =>
    unwrap<Post[]>(api.get(`/posts/user/${userId}`, { params: { page, pageSize: PAGE_SIZE } })),
  get: (id: string) => unwrap<Post>(api.get(`/posts/${id}`)),
  create: (content: string, images: File[], videos: File[]) => {
    const data = new FormData();
    data.append("Content", content);
    images.forEach((f) => data.append("ImageUrls", f));
    videos.forEach((f) => data.append("VideoUrls", f));
    return unwrap<Post>(api.post("/posts", data));
  },
  update: (id: string, content: string) => {
    const data = new FormData();
    data.append("Id", id);
    data.append("Content", content);
    return api.put("/posts", data);
  },
  remove: (id: string) => api.delete(`/posts/${id}`),
};

export const commentsApi = {
  byPost: (postId: string) => unwrap<Comment[]>(api.get(`/comments/post/${postId}`)),
  create: (postId: string, content: string) => unwrap<Comment>(api.post("/comments", { postId, content })),
  update: (id: string, content: string) => api.put("/comments", { id, content }),
  remove: (id: string) => api.delete(`/comments/${id}`),
};

export const likesApi = {
  likePost: (postId: string) => unwrap<LikeResult>(api.post("/likes", { postId })),
  unlikePost: (postId: string) => api.delete("/likes", { params: { postId } }),
  likeComment: (commentId: string) => unwrap<LikeResult>(api.post("/likes", { commentId })),
  unlikeComment: (commentId: string) => api.delete("/likes", { params: { commentId } }),
};

export const followsApi = {
  follow: (followingId: string) => api.post("/follows", { followingId }),
  unfollow: (followingId: string) => api.delete(`/follows/${followingId}`),
  followers: (userId: string) => unwrap<Follow[]>(api.get(`/follows/followers/${userId}`)),
  following: (userId: string) => unwrap<Follow[]>(api.get(`/follows/following/${userId}`)),
};

export const chatApi = {
  conversations: () => unwrap<Conversation[]>(api.get("/chats/conversations")),
  history: (otherUserId: string) => unwrap<ChatMessage[]>(api.get(`/chats/history/${otherUserId}`)),
  send: (receiverId: string, message: string) => unwrap<ChatMessage>(api.post("/chats/send", { receiverId, message })),
};

export const notificationsApi = {
  list: () => unwrap<AppNotification[]>(api.get("/notifications")),
  unreadCount: () => unwrap<number>(api.get("/notifications/unread-count")),
  markRead: (id: string) => api.put(`/notifications/${id}/read`),
  markAllRead: () => api.put("/notifications/read-all"),
};

export const newsApi = {
  list: () => unwrap<News[]>(api.get("/news")),
  get: (id: string) => unwrap<News>(api.get(`/news/${id}`)),
  create: (form: { title: string; content: string; source: string; image: File }) => {
    const data = new FormData();
    data.append("Title", form.title);
    data.append("Content", form.content);
    data.append("Source", form.source);
    data.append("ImageUrl", form.image);
    return unwrap<News>(api.post("/news", data));
  },
  remove: (id: string) => api.delete(`/news/${id}`),
};

export const pollsApi = {
  list: () => unwrap<Poll[]>(api.get("/polls")),
  create: (question: string, options: string[], expiryDate: string) =>
    unwrap<Poll>(api.post("/polls", { question, options, expiryDate })),
  vote: (pollId: string, optionId: string) => unwrap<Poll>(api.post(`/polls/${pollId}/vote/${optionId}`)),
  remove: (id: string) => api.delete(`/polls/${id}`),
};

export const searchApi = {
  search: (q: string) => unwrap<SearchResult>(api.get("/search", { params: { q } })),
  autocomplete: (prefix: string) => unwrap<AutoComplete>(api.get("/search/autocomplete", { params: { prefix } })),
};
