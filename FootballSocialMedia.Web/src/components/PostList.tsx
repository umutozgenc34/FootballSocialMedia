import type { InfiniteData, UseInfiniteQueryResult } from "@tanstack/react-query";
import type { ReactNode } from "react";
import type { Post } from "../api/types";
import { ErrorBox, Spinner } from "./Feedback";
import { InfiniteSentinel } from "./InfiniteSentinel";
import { PostCard } from "./PostCard";

interface Props {
  query: UseInfiniteQueryResult<InfiniteData<Post[]>>;
  empty: ReactNode;
}

export function PostList({ query, empty }: Props) {
  const { data, isLoading, isError, error, hasNextPage, fetchNextPage, isFetchingNextPage } = query;
  if (isLoading) return <Spinner />;
  if (isError) return <ErrorBox error={error} />;

  const posts = data?.pages.flat() ?? [];
  if (posts.length === 0) return <>{empty}</>;

  return (
    <>
      {posts.map((post) => (
        <PostCard key={post.id} post={post} />
      ))}
      <InfiniteSentinel onVisible={() => fetchNextPage()} disabled={!hasNextPage || isFetchingNextPage} />
      {isFetchingNextPage && <Spinner />}
    </>
  );
}
