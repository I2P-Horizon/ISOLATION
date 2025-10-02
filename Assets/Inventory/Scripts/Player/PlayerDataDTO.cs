/*
                        PlayerDataDTO

            - 플레이어 데이터 DTO 클래스
                - Status (능력치) 데이터
                - Position (위치) 데이터
*/
[System.Serializable]
public class PlayerDataDTO
{
    public StatusDTO Status;
    public PositionDTO Position;

    [System.Serializable]
    // 스탯 정보
    public class StatusDTO
    {
        public float maxHp;             // 최대체력
        public float curHp;             // 현재체력
        public float speed;             // 이동속도
        public float rotateSpeed;       // 회전속도
        public float damage;            // 기본 공격력
        public float defense;           // 기본 방어력
        public int gold;                // 보유 골드
    }

    [System.Serializable]
    // 위치 정보
    public class PositionDTO
    {
        public float posX;              
        public float posY;
        public float posZ;
    }
}
