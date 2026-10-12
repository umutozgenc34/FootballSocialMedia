import type { InfiniteData, QueryClient } from "@tanstack/react-query";
import type { Post, SearchResult } from "../api/types";

/**
 * Bir post birden fazla listede (akış, profil, arama, detay) cache'lenmiş olabilir.
 * Beğeni / yorum sayısı / düzenleme gibi değişiklikleri hepsine aynı anda uygular.
 */
export function patchPost(qc: QueryClient, postId: string, patch: (p: Post) => Post) {
  const apply = (p: Post) => (p.id === postId ? patch(p) : p);

  qc.setQueriesData<InfiniteData<Post[]>>({ queryKey: ["posts"] }, (data) =>
    data && "pages" in data ? { ...data, pages: data.pages.map((page) => page.map(apply)) } : data,
  );
  qc.setQueryData<Post>(["post", postId], (p) => (p ? patch(p) : p));
  qc.setQueriesData<SearchResult>({ queryKey: ["search"] }, (data) =>
    data && "posts" in data ? { ...data, posts: data.posts.map(apply) } : data,
  );
}

export function removePost(qc: QueryClient, postId: string) {
  qc.setQueriesData<InfiniteData<Post[]>>({ queryKey: ["posts"] }, (data) =>
    data && "pages" in data ? { ...data, pages: data.pages.map((page) => page.filter((p) => p.id !== postId)) } : data,
  );
  qc.setQueriesData<SearchResult>({ queryKey: ["search"] }, (data) =>
    data && "posts" in data ? { ...data, posts: data.posts.filter((p) => p.id !== postId) } : data,
  );
  qc.removeQueries({ queryKey: ["post", postId] });
}

/** Yeni postu akışların en başına ekler. */
export function prependPost(qc: QueryClient, post: Post) {
  for (const key of [["posts", "feed", "all"], ["posts", "feed", "following"], ["posts", "user", post.userId]]) {
    qc.setQueryData<InfiniteData<Post[]>>(key, (data) =>
      data ? { ...data, pages: [[post, ...(data.pages[0] ?? [])], ...data.pages.slice(1)] } : data,
    );
  }
}
