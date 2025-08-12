using System.Collections.Generic;
using System.Threading;
using UnityEngine;



    public static class SHI_RandomPosCreater
    {
    
    public static Vector3 RandomPos(Vector3 min, Vector3 max, bool IsGround = false)
        {
            if (IsGround) return RandomGroundPos(min, max);
            else return RandomAllPos(min, max);
        }

        private static Vector3 RandomAllPos(Vector3 min, Vector3 max)
        {
            float x = Random.Range(min.x, max.x);
            float z = Random.Range(min.z, max.z);
            float y = 50; // y는 하늘 위에서 시작
            

        return new Vector3(x, y, z);
        }

        private static Vector3 RandomGroundPos(Vector3 min, Vector3 max)
        {
            Vector3 groundPos;
            float maxHeight = 50;
            bool hitGround = false;
            int time = 0;
        LayerMask groundLayer = (1 << 26);
                                    // 무한 루프 방지용 카운트
        
        do
            {
                groundPos = RandomAllPos(min, max);
            // Raycast로 바닥을 찾기
            Ray ray = new Ray(groundPos, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, maxHeight, groundLayer))
                {
                    groundPos.y = maxHeight;
                    hitGround = true;
                    Debug.Log(hitGround);
                }

                time++;
                if (time > 15)
                    break;

            } while (!hitGround);

            return groundPos;
        }

        public static List<Vector3> RandomPosList(Vector3 min, Vector3 max, int num)
        {
            List<Vector3> result = new();
            float divideX = (max.x - min.x) / num;

            for (int i = 0; i < num; i++)
            {
                Vector3 dividedMin = new Vector3(min.x + divideX * i, 30, min.z);
                Vector3 dividedMax = new Vector3(min.x + divideX * (i + 1), 30, max.z);
                result.Add(RandomGroundPos(dividedMin, dividedMax));
            }

            return result;
        }
    }
