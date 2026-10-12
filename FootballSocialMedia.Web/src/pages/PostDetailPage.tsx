import { useQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import { postsApi } from "../api/endpoints";
import { Comments } from "../components/Comments";
import { ErrorBox, Spinner } from "../components/Feedback";
import { PostCard } from "../components/PostCard";

export function PostDetailPage() {
  const { id = "" } = useParams();
  const navigate = useNavigate();
  const { data: post, isLoading, isError, error } = useQuery({
    queryKey: ["post", id],
    queryFn: () => postsApi.get(id),
  });

  return (
    <>
      <header className="page-head">
        <button className="back btn-ghost btn" onClick={() => (history.length > 1 ? navigate(-1) : navigate("/"))}>
          <ArrowLeft size={18} /> Geri
        </button>
        <h1>Gönderi</h1>
      </header>
      {isLoading && <Spinner />}
      {isError && <ErrorBox error={error} />}
      {post && (
        <>
          <PostCard post={post} link={false} detail />
          <Comments post={post} />
        </>
      )}
    </>
  );
}
