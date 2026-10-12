// Backend DTO'larının TypeScript karşılıkları (JSON camelCase döner).

export interface ServiceResult<T> {
  data?: T;
  errorMessage?: string[] | null;
  message?: string | null;
}

export interface TokenDto {
  accessToken: string;
  accessTokenExpiration: string;
  refreshToken: string;
  refreshTokenExpiration: string;
}

export interface UserDto {
  id: string;
  userName: string;
  email: string;
}

export interface UserSummary {
  id: string;
  userName: string;
  profileImageUrl?: string | null;
  favoriteTeam?: string | null;
}

export interface UserProfile {
  id: string;
  userName: string;
  email?: string | null;
  favoriteTeam?: string | null;
  bio?: string | null;
  profileImageUrl?: string | null;
  postCount: number;
  followerCount: number;
  followingCount: number;
  isMe: boolean;
  isFollowedByMe: boolean;
  roles: string[];
  createdTime: string;
}

export interface Post {
  id: string;
  content: string;
  imageUrls: string[];
  videoUrls: string[];
  userId: string;
  username: string;
  profileImageUrl?: string | null;
  favoriteTeam?: string | null;
  likeCount: number;
  commentCount: number;
  isLikedByMe: boolean;
  createdTime: string;
  updatedTime: string;
}

export interface Comment {
  id: string;
  postId: string;
  content: string;
  userId: string;
  username: string;
  profileImageUrl?: string | null;
  likeCount: number;
  isLikedByMe: boolean;
  createdTime: string;
  updatedTime: string;
}

export interface LikeResult {
  id: string;
  likeCount: number;
}

export interface Follow {
  followerId: string;
  followerUserName?: string | null;
  followerProfileImageUrl?: string | null;
  followingId: string;
  followingUserName?: string | null;
  followingProfileImageUrl?: string | null;
  createdTime: string;
}

export interface ChatMessage {
  id: string;
  senderId: string;
  receiverId: string;
  senderName?: string | null;
  receiverName?: string | null;
  message: string;
  timestamp: string;
}

export interface Conversation {
  userId: string;
  userName: string;
  profileImageUrl?: string | null;
  lastMessage: string;
  lastMessageTime: string;
  lastMessageIsMine: boolean;
}

export type NotificationType = "Like" | "Comment" | "Follow" | "Message";

export interface AppNotification {
  id: string;
  userId: string;
  senderId: string;
  senderUserName?: string | null;
  type: NotificationType;
  content: string;
  postId?: string | null;
  commentId?: string | null;
  isRead: boolean;
  createdAt: string;
}

export interface News {
  id: string;
  title: string;
  content: string;
  source: string;
  imageUrl?: string | null;
  createdTime: string;
  updatedTime: string;
}

export interface PollOption {
  id: string;
  optionText: string;
  voteCount: number;
}

export interface Poll {
  id: string;
  question: string;
  options: PollOption[];
  totalVotes: number;
  myVoteOptionId?: string | null;
  isExpired: boolean;
  expiryDate: string;
  createdTime: string;
}

export interface SearchResult {
  users: UserSummary[];
  posts: Post[];
  engine: "elasticsearch" | "sql";
}

export interface AutoComplete {
  users: UserSummary[];
  posts: string[];
}
